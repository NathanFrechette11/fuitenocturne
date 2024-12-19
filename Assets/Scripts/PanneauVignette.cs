using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// #tp4 Classe a ajouter sur le prefab des panneau vignette.
/// Elle permet d'aller chercher son image afin de pouvoir y mettre celle de l'objet de l'inventaire
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class PanneauVignette : MonoBehaviour
{
    // definit l'image de la vignette que l'on donne dans l'inspecteur
    [SerializeField] Image _image;

    /// <summary>
    /// fonction qui change l'image original de la vignette par celle des objets de l'inventaire qui sont envoyer
    /// et qui instentie la vignette avec sa nouvelle image
    /// </summary>
    /// <param name="image">image de l'item de l'inventaire a fournir pour changer celle de la vignette</param>
    public void AfficherInventaire(Sprite image)
    {
        _image.sprite = image;
        Instantiate(this, Niveau.instance.zoneInventaire.transform);
    }
}
