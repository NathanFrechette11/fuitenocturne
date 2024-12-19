using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// #tp3 Classe qui permet de créer un asset qui va gerer la navigation dans le projet.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

[CreateAssetMenu(fileName = "MaNavigation", menuName = "Navigation")]
public class SONavigation : ScriptableObject
{
    // donne les donnees du perso afin que les bonus soient reinitialiser quand le joueur quitte le niveau
    [SerializeField] SOPerso _donneesPerso;

    // #synthese donne les donnees du poitage afin que le nombre de points accumule soit reinitialiser quand le joueur quitte le niveau
    [SerializeField] SOPointage _donneesPointage;

    /// <summary>
    /// quand le joueur passe de la scene du menu principale au niveau, cette fonction est appeler pour ammener le joueur au niveau
    /// </summary>
    public void Jouer()
    {
        _donneesPerso.Initialiser();
        AllerSceneSuivante();
    }

    /// <summary>
    /// quand le joueur passe a travers la porte dans le niveau, cette fonction ammene au magasin et enleve les bonus au joueur
    /// </summary>
    public void EntrerBoutique()
    {
        _donneesPerso.InitialiserBonus();
        AllerSceneSuivante();
    }

    /// <summary>
    /// fait sortir le joueur de la boutique pour le ramener au niveau quand celui ci appuie sur le bouton jouer
    /// </summary>
    public void SortirBoutique()
    {
        // Niveau.instance.ModifierTailleNiveau();
        AllerScenePrecedente();
    }

    /// <summary>
    /// fonction qui permet de charger la scene qui est juste apres la scene active dans la liste des scenes du projet
    /// </summary>
    public void AllerSceneSuivante()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// fonction qui permet de charger la scene qui est juste avant la scene active dans la liste des scenes du projet
    /// </summary>
    public void AllerScenePrecedente()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// #tp4 fonction qui permet de charger la scene qui se nomme Generique
    /// </summary>
    public void AllerSceneGenerique()
    {
        SceneManager.LoadScene("Generique");
    }

    /// <summary>
    /// #tp4 fonction qui permet de charger la scene qui se nomme Menu
    /// </summary>
    public void AllerSceneMenu()
    {
        // #synthese changer les variables pour utiliser ceux fournit dans ce script, et non ceux de niveau
        _donneesPerso.Initialiser();
        _donneesPerso.InitialiserBonus();
        _donneesPointage.Initialiser();
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// #tp4 fonction qui permet de charger la scene qui se nomme TableauScores
    /// </summary>
    public void AllerSceneScores()
    {
        SceneManager.LoadScene("TableauScores");
    }

    /// <summary>
    /// #synthese fonction qui permet de charger la scene qui se nomme Explications
    /// </summary>
    public void AllerSceneExplications()
    {
        SceneManager.LoadScene("Explications");
    }
}
