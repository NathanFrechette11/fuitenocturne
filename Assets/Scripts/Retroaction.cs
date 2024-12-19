using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// #tp3 Classe a ajouter sur le prefab du champ de retroaction.
/// Elle permet au champ de changer son texte et de se detruire
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Retroaction : MonoBehaviour
{
    // variable ou on definit le champ de texte dans l'inspecteur
    [SerializeField] TextMeshProUGUI _champRetroaction;

    // #synthese ajout du parametre texte dans le summary
    /// <summary>
    /// fonction qui recoit un texte de la part de la classe qui la appeler pour l'afficher dans son champ de texte
    /// </summary>
    /// <param name="texte">texte a fournir pour qu'il puisse etre afficher dans le champ de texte de la retroaction</param>
    public void ChangerTexte(string texte)
    {
        _champRetroaction.text = texte;
    }

    /// <summary>
    /// fonction appeler une fois que l'animation du champ de texte est fini, detruit le champ de texte
    /// </summary>
    public void Detruire()
    {
        Destroy(gameObject);
    }
}
