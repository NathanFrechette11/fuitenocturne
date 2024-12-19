using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp3 Classe a ajouter sur le prefab de l'argent.
/// Elle permet à l'argent de donner des points quand le joueur la rammasse, fait jouer son systeme de particules,
/// augmente le montant d'argent que le joueur possede, et ce detruit.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Argent : MonoBehaviour
{
    // #synthese variable qui permet de definir le son de l'argent
    [SerializeField] AudioClip _sonArgent;
    // #synthese variable qui permet de definir l'audio source de l'argent
    AudioSource _audio;

    // variables qui permettent d'aller chercher le collider ainsi que le spriteRenderer de l'argent
    SpriteRenderer _sr;
    Collider2D _coll;
    // variable qui définit le nombre de points que le joueur gagne en rammassant l'argent
    int nbPoints = 50;
    // variable qui permet d'aller chercher le systeme de particule
    ParticleSystem _part;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // #synthese va chercher le composant audio source de l'argent
        _audio = GetComponent<AudioSource>();

        // va chercher toutes les composantes mentionné plus haut et les définit à ces variables
        _coll = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _part = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Quand le joueur entre en contact avec l'argent, il n'est plus possible d'interagir avec, change
    /// son opacité pour montrer qu'on ne peut plus interagir, affiche un texte ou le perso la touché,
    /// donne les points, augmente l'argent du joueur et ce détruit
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // si cest le joueur qui est en contact avec l'argent...
        if(other.tag=="Joueur")
        {
            // #synthese on joue le son de l'argent
            _audio.PlayOneShot(_sonArgent);

            // désactive le collider de l'argent
            _coll.enabled=false;
            // le rend completement transparent pour faire comme s'il n'existait plus
            _sr.color = new Color(1,1,1,0f);
            // fait jouer le systeme de particule
            _part.Play();
            // augmente de 1 le nombre d'argent que le joueur possede
            Niveau.instance.donneesPerso.argent++;
            // affiche le montant d'argent que le joueur possede dans la console
            Debug.Log("Argent: " + Niveau.instance.donneesPerso.argent);
            // donne le nombre de points définit plus haut dans le scriptable object de pointage
            Niveau.instance.donneesPointage.pointage = Niveau.instance.donneesPointage.pointage + nbPoints;
            // affiche le champ de texte la ou le joueur a ramasser largent
            Niveau.instance.AfficherTexteRetroactif("Argent collecté");

            // #synthese avant était appeller par "":("Coroutine..."), maintenant est par (): (Coroutine...())
            // commence la coroutine pour que l'argent puisse etre vraiment detruit
            StartCoroutine(CoroutineDetruire());
        }
    }

    /// <summary>
    /// Coroutine qui attend une seconde le temps que les particules finissent de jouer avant de détruire l'argent
    /// </summary>
    IEnumerator CoroutineDetruire()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
