using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// #tp3 Classe a mettre sur le prefab des panneau des objets.
/// Elle permet aux panneaux d'aller chercher les donnees des objets qui leur sont fournie dans l'inspecteur et de les affiche dans
/// ces champs de texte ainsi que dans son champ image
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class PanneauObjet : MonoBehaviour
{
    // permet d'aller chercher les donnees dont la classe aura besoin, et rend public ceux des objets
    [Header("LES DONNÉES")]
    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] SOObjet _donnees;
    public SOObjet donnees => _donnees;

    // permet de definir tout les champs qui afficheront les donnees des objets dans le panneau
    [Header("LES CONTENEURS")]
    [SerializeField] TextMeshProUGUI _champNom;
    [SerializeField] TextMeshProUGUI _champPrix;
    [SerializeField] TextMeshProUGUI _champDescription;
    [SerializeField] Image _image;
    [SerializeField] CanvasGroup _canvasGroup;

    void Start()
    {
        // appel la fonction pour mettre a jour les informations de l'objet dans le panneau
        MettreAJourInfos();
        // abonne la fonction a levenement miseajour qui se situe dans le SOPerso
        _donneesPerso.evenementMiseAJour.AddListener(MettreAJourInfos);
    }

    /// <summary>
    /// fonction qui permet de mettre a jour tout les champs de texte du panneau ainsi que le champ image afin
    /// d'y fournir les informations qui se trouvent dans les donnees de l'objet fournie dans l'inspecteur
    /// </summary>
    private void MettreAJourInfos()
    {
        _champNom.text = _donnees.nom;
        _champPrix.text = _donnees.prix + " $";
        _champDescription.text = _donnees.description;
        _image.sprite = _donnees.sprite;
        // appel la fonction qui verifie si le joueur peut se permettre d'acheter l'objet
        GererDispo();
    }

    /// <summary>
    /// fonction qui verifie si le joueur a asser d'argent dans ses poches pour acheter les items du magasin
    /// </summary>
    void GererDispo()
    {
        // si le montant dargent du perso est plus grand que le prix de lobjet, bool aassezdargent devient true
        bool aAssezArgent = _donneesPerso.argent >= _donnees.prix;
        // sil est true...
        if(aAssezArgent)
        {
            // active les canvas pour que le joueur puisse interagir avec pour acheter l'objet
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1;
        }
        // sinon...
        else
        {
            // les canvas sont désactiver et le joueur ne peut pas interagir avec
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0.5f;
        }
    }

    /// <summary>
    /// fonction qui permet au joueur d'acheter les objets de la boutique et envoie les donnees de lobjet au SOPerso
    /// </summary>
    public void Acheter()
    {
        _donneesPerso.Acheter(_donnees);
    }
}