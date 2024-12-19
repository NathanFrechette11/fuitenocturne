using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// #tp3 Classe a ajouter sur le prefab de la porte.
/// Elle permet à la porte d'etre débarrer et d'emmener le joueur a la scene boutique.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Porte : MonoBehaviour
{
    // #synthese definit une variable ou on va fournir l'image que la porte devrait avoir quand elle est ouverte
    [SerializeField] Sprite _porteOuverte;

    // variables qui permettent d'aller chercher le collider ainsi que le spriteRenderer de la porte
    SpriteRenderer _sr;
    Collider2D _coll;

    // Start is called before the first frame update
    void Start()
    {
        // va chercher le collider et le sprite renderer de la porte et les définit à ces variables et désactive le collider de la porte
        _coll = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _coll.enabled = false;
        // abonne sa fonction Deverrouiller a l'evenement définit dans le script du niveau
        Niveau.instance.deverrouiller.AddListener(Deverrouiller);
    }

    /// <summary>
    /// fonction qui est appeler quand la clé est ramassé et qu'elle fait partir l'évenement deverrouiller
    /// </summary>
    void Deverrouiller()
    {
        // réactive le collider de la porte et change son sprite pour montrer qu'elle est ouverte
        _coll.enabled = true;

        // #synthese change l'image de la porte fermee par celle de la porte ouverte
        _sr.sprite = _porteOuverte;
    }

    /// <summary>
    /// quand le joueur entre en contact avec la porte déverrouillé, celle ci l'emmene dans la boutique
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // si le joueur touche a la porte...
        if(other.tag == "Joueur")
        {
            // #synthese avant était appeller par "":("Coroutine..."), maintenant est par (): (Coroutine...())
            // #tp4 fait partir la coroutine qui permet de quitter la scene
            StartCoroutine(CoroutineQuitterScene());
        }
    }

    /// <summary>
    /// #tp4 Definit une coroutine qui "detruit" le perso, joue son son, attends quelques
    /// secondes et active le bouton qui permet de changer de scene
    /// </summary>
    /// <returns>attends pour une durée de 3 secondes et demi</returns>
    IEnumerator CoroutineQuitterScene()
    {
        // #tp4 appelle la fonction dans niveau qui desactive le perso afin qu'il ne se promene pas quand le niveau est fini
        Niveau.instance.DetruirePerso();
        // #tp4 envoie le nom du son a jouer a la classe niveau
        Niveau.instance.JouerSon("porte");
        // #tp4 definit que le niveau est fini
        Niveau.instance.finNiveau = true;
        // #tp4 attend pendant 3 secondes et demi le temps que le son de la porte joue
        yield return new WaitForSeconds(3.5f);
        // #tp4 arrete la musique de la cle rammassé
        GestionnaireAudio.instance.ChangerEtatLecturePiste(TypePiste.MusiqueEvenA, false);
        // #tp4 active le bouton qui permet au joueur de s'en aller dans la boutique
        Niveau.instance.activerBouton = true;
    }
}
