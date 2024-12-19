using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// #tp3 Classe a ajouter sur le prefab des pillules.
/// Elle permet aux pillules de donner au joueur un bonus de saut quand ceux ci sont activer et que le joueur les 
/// rammasse, donne des points pour les avoir rammasser et ce detruit.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Pillule : MonoBehaviour
{
    // #synthese variable qui permet de definir le son de la pillule
    [SerializeField] AudioClip _sonPillule;
    // #synthese variable qui permet de definir l'audio source de la pillule
    AudioSource _audio;

    // variables qui permettent d'aller chercher le collider ainsi que le spriteRenderer des pillules
    SpriteRenderer _sr;
    Collider2D _coll;
    // définit le nombre de points que le joueur gagne en ramassant une pillule
    int nbPoints = 30;
    // variables qui permettront d'acceder a différentes fonctionnalité du systeme de particules
    ParticleSystem _part;

    // Start is called before the first frame update
    void Start()
    {
        // #synthese va chercher le composant audio source de la pillule
        _audio = GetComponent<AudioSource>();

        // va chercher toutes les composantes mentionné plus haut et les définit à ces variables
        _part = GetComponent<ParticleSystem>();
        _coll = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        // met la pillule a moitier transparent pour dire qu'elle n'est pas activé
        _sr.color = new Color(1,1,1,0.3f);
        // desactive son collider pour pas que le joueur la collecte
        _coll.enabled = false;
        // s'abonne a levenement activation qui se trouve dans la classe niveau
        Niveau.instance.activation.AddListener(ActiverBonus);
    }

    /// <summary>
    /// une fois que l'activateur a été activer, celui ci appelle l'evenement activation qui permet d'activer
    /// les pillules qui se trouvent dans le niveau
    /// </summary>
    public void ActiverBonus()
    {
        // rend l'objet completement visible pour montrer qu'il est interagissable, et reactive son collider
        _sr.color = new Color(1,1,1,1f);
        _coll.enabled = true;
    }

    /// <summary>
    /// quand le joueur rentre en contact avec la pillule, celle ci fait jouer ses particules, commence une
    /// coroutine pour ce detruire, donne des points au joueur, et se desactive
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // si le joueur entre en contact avec la pillule...
        if(other.tag=="Joueur")
        {
            // #synthese on joue le son de la pillule
            _audio.PlayOneShot(_sonPillule);

            // fait jouer ses particules
            _part.Play();

            // #tp4 quand le joueur ramasse une pillule, on fait partir la musique de bonus de scene
            GestionnaireAudio.instance.ChangerEtatLecturePiste(TypePiste.MusiqueEvenB, true);
            // #tp4 affiche un message en jaune dans la console comme quoi un bonus a été ramassé, donc la musique part
            Debug.Log("<color=yellow>Un Bonus a été ramassé, ajout d'une mélodie</color>");
            // #tp4 on diminue le nombre de pillules qui sont sur la scene
            Niveau.instance.nbPillulesSurScene--;

            // #synthese avant était appeller par "":("Coroutine..."), maintenant est par (): (Coroutine...())
            // commence la coroutine pour ce detruire
            StartCoroutine(CoroutineBonus());

            // donne les points definit plus haut dans le scriptable object pointage
            Niveau.instance.donneesPointage.pointage = Niveau.instance.donneesPointage.pointage + nbPoints;
            // affiche le champ de texte la ou le perso a ramasser le bonus
            Niveau.instance.AfficherTexteRetroactif("Bonus collecté");
            // se met completement invisible et desactive son collider pour faire comme si elle etait detruite
            _sr.color = new Color(1,1,1,0f);
            _coll.enabled=false;
        }
    }

    /// <summary>
    /// coroutine qui donne le bonus au joueur quand elle est appeler, et apres 5 secondes,
    /// retire le bonus et detruit la pillule
    /// </summary>
    IEnumerator CoroutineBonus()
    {
        yield return new WaitForSeconds(5f);

        // #synthese affiche un message en jaune dans la console comme quoi la duree du bonus est finie
        Debug.Log("<color=yellow>Le délais du bonus a expirer, retire la mélodie</color>");

        // #tp4 arrete la musique une fois que la duree maximal du bonus a été atteinte
        GestionnaireAudio.instance.ChangerEtatLecturePiste(TypePiste.MusiqueEvenB, false);
        
        Destroy(gameObject);
    }
}
