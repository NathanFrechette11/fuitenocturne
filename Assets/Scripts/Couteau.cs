using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// #synthese Classe a ajouter sur les prefabs des couteau (gauche et droite).
/// Elle permet aux couteaux de ce déplacer et de ce detruire quand ils foncent dans un objet
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>
public class Couteau : MonoBehaviour
{
    // definit la vitesse a laquelle le couteau devrait ce déplacer
    [SerializeField] float _vitesse = 4.5f;


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // déplace le couteau (vers la droite par defaut) selon le temps qui s'écoule et la vitesse du couteau (si negative deplace a gauche)
        transform.position += transform.right * Time.deltaTime * _vitesse;
    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        // detruit le couteau quand il entre en collision avec un autre objet
        Destroy(gameObject);
    }
}
