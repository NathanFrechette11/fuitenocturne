using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// #tp4 Classe a ajouter sur le prefab des panneau de pointage du tableau des scores.
/// Elle permet de definir plusieur variables afin d'afficher les bonnes valeurs dans le tableau des scores
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class PanneauPointage : MonoBehaviour
{
    // definit les champs des noms et cree un getteur afin que d'autre script y ait acces
    [SerializeField] TextMeshProUGUI _champNom;
    public TextMeshProUGUI champNom => _champNom;
    // definit les champs des pointages et cree un getteur afin que d'autre script y ait acces
    [SerializeField] TextMeshProUGUI _champPointage;
    public TextMeshProUGUI champPointage => _champPointage;
    // definit le fond de la vignette et cree un getteur afin que d'autre script y ait acces
    [SerializeField] Image _fond;
    public Image fond => _fond;
    // definit la place a laquelle la vignette est place dans le tableau et cree un getter setter afin d'y changer la valeur ailleur
    [SerializeField] int _place;
    public int place
    {
        get => _place;
        set
        {
            _place = value;
        }
    }
    // definit les elements qui permettent au joueur de s'enregistrer dans le tableau des scores
    [SerializeField] GameObject _groupeInput;

    // #synthese definit le inputfield dans lequel le joueur rentre son nom
    [SerializeField] TMP_InputField _inputField;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // #synthese fait en sorte que le joueur ne peux pas mettre plus que 9 caractheres dans le champ de texte
        _inputField.characterLimit = 9;
    }

    /// <summary>
    /// fonction qui permet de desactiver le boutton enregistrer ainsi que le inputfield de chaque vignette
    /// </summary>
    public void DesactiverInput()
    {
        _groupeInput.SetActive(false);
    }

    /// <summary>
    /// fonction qui permet d'activer le boutton enregistrer ainsi que le inputfield de la vignette de score du joueur actuelle
    /// </summary>
    public void ActiverInput()
    {
        _groupeInput.SetActive(true);
    }
}
