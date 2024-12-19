using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// #tp3 Classe a mettre sur le prefab de la boutique.
/// Elle permet a la boutique d'afficher le montant d'argent que le joueur possede ainsi que le niveau quil
/// est rendu, reinitialise les donnees du joueur si le joueur quitte le jeu en étant dans la boutique
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Boutique : MonoBehaviour
{
    // variables ou lon peut fournir les champs de textes nessecaires ainsi que les donnees du personnage dans l'inspecteur
    [SerializeField] SOPerso _donneesPerso;

    // #tp4 definit la variable des donnees de pointage qui sera donner dans l'inspecteur
    [SerializeField] SOPointage _donneesPointage;

    // #synthese definit les données de la difficulté du mode de jeu
    [SerializeField] SODifficulte _donneesDifficulte;

    [SerializeField] TextMeshProUGUI _champNiveau;
    [SerializeField] TextMeshProUGUI _champArgent;

    // bool qui permet de faire augmenter le niveau quand le joueur quitte la boutique
    bool _estEnPlay = true;
    void Awake()
    {
        // appelle la fonction pour mettre a jour les information
        MettreAJourInfos();
        // abonne cette fonction a levenement mise a jour qui se trouve dans le SOPerso
        _donneesPerso.evenementMiseAJour.AddListener(MettreAJourInfos);
    }

    /// <summary>
    /// fonction pour mettre les informations des champs de texte de la boutique a jour
    /// </summary>
    private void MettreAJourInfos()
    {
        _champNiveau.text = "Niveau " + _donneesPerso.niveau;
        _champArgent.text = _donneesPerso.argent + " $";
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        // quand le joueur quitte le jeu, le bool estenplay devient faux
        _estEnPlay = false;
        // les donnees du personnage sont remis a zero
        _donneesPerso.Initialiser();

        // #tp4 permet de remettre a 0 les valeurs des bonus du personnage sil quitte le jeu a partir de la boutique
        _donneesPerso.InitialiserBonus();
        // et le pointage est aussi remit a zero
        _donneesPointage.Initialiser();

        // #synthese remet le mode de jeu a normal (met le modeDifficile du SODifficulete a false si le joueur quitte le jeu)
        _donneesDifficulte.modeDifficile = false;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        // quand le joueur quitte la boutique et quelle se detruit, enleve tout abonnement a levenement miseajour
        _donneesPerso.evenementMiseAJour.RemoveAllListeners();
        // si le joueur quitte pour aller au niveau, le niveau quil est rendu augmente
        if(_estEnPlay) _donneesPerso.niveau++;
    }
}