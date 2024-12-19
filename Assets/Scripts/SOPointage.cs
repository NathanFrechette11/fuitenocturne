using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// #tp3 Classe qui permet de créer un scriptable object pour le pointage du jeu.
/// Elle additionne tout les scores qui lui sont envoyer et les affiches dans la console
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

// permet de creer l'asset qui va contenir toutes les donnees
[CreateAssetMenu(fileName = "Pointage", menuName = "Pointage")]
public class SOPointage : ScriptableObject
{
    // définit un evenement qui va faire la mise a jour du pointage dans la console et prepare son getteur
    UnityEvent _evenementMiseAJour = new UnityEvent();
    public UnityEvent evenementMiseAJour => _evenementMiseAJour;

    // definit la variable de pointage qui recevra tout les points, ainsi que son getteur et sa valeur initial
    [SerializeField, Range(0,1000)] int _pointageIni = 0;
    [SerializeField, Range(0,1000)] int _pointage = 0;
    public int pointage
    {
        get => _pointage;
        set
        {
            _pointage = Mathf.Clamp(value, 0, int.MaxValue);
            // appel levenement mise a jour pour afficher le bon pointage dans la console
            _evenementMiseAJour.Invoke();
        } 
    }

    /// <summary>
    /// fonction qui permet de remettre le pointage a 0 quand le joueur quite le jeu
    /// </summary>
    public void Initialiser()
    {
        _pointage = _pointageIni;
        Debug.Log("Pointage: " + _pointage);
    }
}
