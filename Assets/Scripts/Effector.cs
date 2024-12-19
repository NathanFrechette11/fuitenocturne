using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp3 Classe a ajouter sur le prefab des effectors.
/// Elle permet au effector de faire jouer leur systeme de particule.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Effector : MonoBehaviour
{
    // variable qui permettra d'acceder a divers fonction du systeme de particules
    ParticleSystem _part;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // va chercher le systeme de particule afin d'y avoir acces
        _part = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// quand le joueur entre en contact avec l'effector, celui ci fait jouer son systeme de particule
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // si le joueur entre en contact avec l'effector...
        if(other.tag=="Joueur")
        {
            // active le systeme de particule
            _part.Play();
        }
    }
}
