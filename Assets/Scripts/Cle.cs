using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp3 Classe a ajouter sur la clé.
/// Elle fait en sorte que quand le joueur touche la clé, celle ci débare la porte, fait jouer son système de 
/// particule et détruit la clé après une seconde
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Cle : MonoBehaviour
{
    // définit le nombre de points que le joueur gagne en ramassant la clé
    int nbPoints = 150;
    // variables qui permettront d'acceder a différentes fonctionnalité du systeme de particules
    ParticleSystem _part;

    // #synthese definit le sprite renderer de la cle
    SpriteRenderer _sr;
    // #synthese definit le collider de la cle
    Collider2D _coll;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // va chercher le systeme de particule
        _part = GetComponent<ParticleSystem>();

        // #synthese va chercher la composante SpriteRenderer
        _sr = GetComponent<SpriteRenderer>();
        // #synthese va chercher la composante Collider2D
        _coll = GetComponent<Collider2D>();
    }

    /// <summary>
    /// quand le joueur rentre en contact avec la clé, celle ci joue ses particules, rajoute des points
    /// au systeme de score, dévérouille la porte et ce détruit
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // si le contact se fait avec le joueur...
        if(other.tag == "Joueur")
        {

            // #synthese desactive le collider de la cle afin que le joueur ne la "ramasse" pas 2 fois (ou plus)
            _coll.enabled=false;
            // #synthese rend la cle transparente pour faire comme si elle avait été ramassé
            _sr.color = new Color(1,1,1,0f);

            // #tp4 envoie le nom du son a jouer dans la classe niveau
            Niveau.instance.JouerSon("cle");

            // joue les particules
            _part.Play();
            // dévérouille la porte grâce à l'utilisation d'un événement
            Niveau.instance.deverrouiller.Invoke();
            // fait apparaitre un texte ou le perso a toucher la clé
            Niveau.instance.AfficherTexteRetroactif("Porte déverrouillé");
            // donne le nombre de point définit plus haut dans le scriptable object de pointage
            Niveau.instance.donneesPointage.pointage = Niveau.instance.donneesPointage.pointage + nbPoints;

            // #tp4 faire partir la musique de cle ramasser
            GestionnaireAudio.instance.ChangerEtatLecturePiste(TypePiste.MusiqueEvenA, true);
            // #tp4 affiche un message en vert dans la console comme quoi la cle a ete ramasser alors la musique part
            Debug.Log("<color=green>La clé a été ramassé, ajout de plus de base</color>");

            // #synthese avant était appeller par "":("Coroutine..."), maintenant est par (): (Coroutine...())
            // commence la coroutine pour détruire la clé
            StartCoroutine(CoroutineDetruire());
        }
    }

    /// <summary>
    /// Coroutine qui attend une seconde le temps que les particules finissent de jouer avant de détruire la clé
    /// </summary>
    IEnumerator CoroutineDetruire()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
