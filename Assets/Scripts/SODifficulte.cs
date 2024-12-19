using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #synthese Classe qui permet de créer un asset qui va gerer la difficulté du mode de jeu.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>
[CreateAssetMenu(fileName = "Difficulté", menuName = "Difficulté")]

public class SODifficulte : ScriptableObject
{
    // definit un bool qui permet de savoir si le mode de jeu est en mode difficile ou non
    // et creer un getteur/setteur afin d'y acceder ailleurs
    [SerializeField] bool _modeDifficile = false;
    public bool modeDifficile
    {
        get => _modeDifficile;
        set
        {
            _modeDifficile = value;
        }
    }

    /// <summary>
    /// fonction qui permet de changer les valeurs de certaines variables que le mode de jeu devrait changer quand il est difficile
    /// </summary>
    public void ChangerDifficulte()
    {
        // si le jeu est en mode difficile...
        if(_modeDifficile)
        {
            // le nombre d'ennemies qui devrait apparaitre est égal au nombre de salles
            Niveau.instance.nbEnnemis = Niveau.instance.nbSalles;
            // le personnage n'a qu'une seule vie
            Niveau.instance.donneesPerso.vie = 1;
            // definit la valeur de la variable probabiliteArgent de Niveau avec des valeurs plus basses que normal
            Niveau.instance.probabiliteArgent = UnityEngine.Random.Range(12, 28);
            // definit la valeur de la variable probabiliteBonus de Niveau avec des valeurs plus basses que normal
            Niveau.instance.probabiliteBonus = UnityEngine.Random.Range(4, 15);
        }
        // sinon...
        else
        {
            // le nombre d'ennemies qui devrait apparaitre est égal a la moitié du nombre de salles
            Niveau.instance.nbEnnemis = Niveau.instance.nbSalles / 2;
            // definit la valeur de la variable probabiliteArgent de Niveau avec les valeurs normal
            Niveau.instance.probabiliteArgent = UnityEngine.Random.Range(25, 46);
            // definit la valeur de la variable probabiliteBonus de Niveau avec les valeurs normal
            Niveau.instance.probabiliteBonus = UnityEngine.Random.Range(8, 20);
        }
    }
}