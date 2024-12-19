using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp3 Classe a ajouter sur le prefab de l'activateur.
/// Elle permet d'activer tout les bonus qui sont présent un peu partout dans le niveau.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Activateur : MonoBehaviour
{
    // #synthese variable qui permet de definir le son de l'activateur
    [SerializeField] AudioClip _sonActivateur;
    // #synthese variable qui permet de definir l'audio source de l'activateur
    AudioSource _audio;

    // variables qui permettent d'aller chercher le collider ainsi que le spriteRenderer de l'activateur
    Collider2D _coll;
    SpriteRenderer _sr;
    // variable qui définit le nombre de points que le joueur gagne en activant l'activateur
    int nbPoints = 100;
    // variable qui permet d'aller chercher le systeme de particule
    ParticleSystem _part;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // #synthese va chercher le composant audio source de l'activateur
        _audio = GetComponent<AudioSource>();

        // va chercher toutes les composantes mentionné plus haut et les définit à ces variables
        _part = GetComponent<ParticleSystem>();
        _sr = GetComponent<SpriteRenderer>();
        _coll = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Quand le joueur entre en contact avec l'activateur, il n'est plus possible d'interagir avec, change
    /// son opacité pour montrer qu'on ne peut plus interagir, affiche un texte ou le perso la touché,
    /// donne les points et active tout les bonus
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // si c'est le perso qui a touché à l'activateur...
        if(other.tag == "Joueur")
        {
            // #synthese on joue le son de l'activateur
            _audio.PlayOneShot(_sonActivateur);

            // l'activateur n'est plus interagissable
            _coll.enabled=false;
            // il fait jouer son systeme de particule
            _part.Play();
            // devient plus translucide pour montrer l'incapacité d'interagir avec
            _sr.color = new Color(1,1,1,0.3f);
            // affiche la boite de texte comme quoi qu'il est activé
            Niveau.instance.AfficherTexteRetroactif("Bonus activés");
            // donne le nombre de points qui est définit plus haut au scriptable object de pointage
            Niveau.instance.donneesPointage.pointage = Niveau.instance.donneesPointage.pointage + nbPoints;
            // et active tout les bonus du niveau à l'aide d'un évènement
            Niveau.instance.activation.Invoke();
        }
    }
}
