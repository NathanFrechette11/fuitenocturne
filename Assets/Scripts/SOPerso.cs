using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// #tp3 Classe qui pernet de creer un asset qui contiendra toute les donnees du personnage.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

[CreateAssetMenu(fileName = "Perso", menuName = "Perso")]
public class SOPerso : ScriptableObject
{
    // definit tout les bonus que le personnage peut avoir ainsi que de creer des getteurs pour pouvoir les activer ailleur
    bool _doubleSaut = false;
    public bool doubleSaut => _doubleSaut;
    bool _vitesseEleve = false;
    public bool vitesseEleve => _vitesseEleve;
    bool _superSaut = false;
    public bool superSaut { get => _superSaut; set => _superSaut = value; }

    // definit le niveau ainsi que l'argent que le personnage a ainsi que leur valeur initial
    [Header("Valeurs initiales")]
    [SerializeField, Range(1,5)] int _niveauIni = 1;
    [SerializeField, Range(0,500)] int _argentIni = 5;

    // #tp4 Ajout de variables initiales pour la vie du joueur et le temps restant
    [SerializeField, Range(0,3)] int _vieIni = 3;
    [SerializeField, Range(0,60)] int _tempsIni = 60;


    [Header("Valeurs actuelles")]
    [SerializeField, Range(1,5)] int _niveau = 1;
    [SerializeField, Range(0,500)] int _argent = 5;

    // #tp4 Ajout de variables pour la vie du joueur et le temps restant
    [SerializeField, Range(0,3)] int _vie = 3;
    [SerializeField, Range(0,60)] int _temps = 60;



    // cree des getter de ces valeurs la afin que d'autre script puisse afficher le montant d'argent, l'augmenter, baisse, afficher niveau
    public int niveau
    {
        get => _niveau;
        set 
        {
            _niveau = Mathf.Clamp(value, 1, int.MaxValue);
            _evenementMiseAJour.Invoke();
        }
    }
    public int argent
    {
        get => _argent;
        set
        {
            _argent = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();
        } 
    }

    //#tp4 cree des getter pour la vie et le temps afin que d'autre script puisse afficher, augmenter, ou baisse la vie du perso et le temps
    public int vie
    {
        get => _vie;
        set
        {
            _vie = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();
        } 
    }
    public int temps
    {
        get => _temps;
        set
        {
            _temps = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();
        } 
    }


    // creer levenement miseajour afin de mettre a jour les valeurs de niveau et du montant d'argent
    UnityEvent _evenementMiseAJour = new UnityEvent();
    public UnityEvent evenementMiseAJour => _evenementMiseAJour;

    // creer une liste d'objet qui recevra tout les objets que le joueur aura acheté
    List<SOObjet> _lesObjets = new();

    /// <summary>
    /// remet les variables de niveau et du montant d'argent a leur valeur initial
    /// </summary>
    public void Initialiser()
    {
        _niveau = _niveauIni;
        _argent = _argentIni;

        // #tp4 remet les variables de vie et de temps a leur valeur initial
        _vie = _vieIni;
        _temps = _tempsIni;

        _lesObjets.Clear();
    }

    /// <summary>
    /// enleve tout les bonus que le joueur avait une fois qu'il quitte le niveau
    /// </summary>
    public void InitialiserBonus()
    {
        _lesObjets.Clear();
        _doubleSaut = false;
        _vitesseEleve = false;
        _superSaut = false;
    }

    /// <summary>
    /// quand le joueur appuie sur le bouton acheter et qu'il a asser d'argent, l'argent diminue selon le prix
    /// de l'objet achete et celui se rajoute dans la liste des objets du perso
    /// </summary>
    public void Acheter(SOObjet donneesObjet)
    {
        // si le joueur a asser d'argent...
        if(_argent >= donneesObjet.prix)
        {
            // enleve le montant de lobjet a la somme total de l'argent du joueur
            argent -= donneesObjet.prix;
            // verifie si l'objet acheter permet de donner un bonus au joueur
            if(donneesObjet.donneDroitDoubleSaut) _doubleSaut = true;
            if(donneesObjet.donneDroitVitesseEleve) _vitesseEleve = true;
            // ajoute l'objet a la liste des objets du personnage
            _lesObjets.Add(donneesObjet);
            // appel la fonction pour afficher l'inventaire du joueur dans la console
            AfficherInventaire();
        }
    }

    /// <summary>
    /// prend tout les objets qui se trouve dans la liste des objets achete, et affiche leur nom dans la console
    /// </summary>
    void AfficherInventaire()
    {
        // declare une variable qui servira a contenir tout les noms des objets dans l'inventaire
        string inventaire = "";
        // pour chaque objet dans la liste d'objets...
        foreach(SOObjet objet in _lesObjets)
        {
            // si l'inventaire nest pas deja vide, on rajoute une virgule pour separer les items de l'inventaire
            if(inventaire != "") inventaire += ", ";
            // on rajoute le nom de lobjet dans l'inventaire
            inventaire += objet.nom;
        }
        // et on affiche l'inventaire dans la console
        Debug.Log("Inventaire du perso: "+inventaire);
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate()
    {
        // appel levenement miseajour a chaque fois que quelque chose change dans l'inspecteur
        _evenementMiseAJour.Invoke();
    }

    /// <summary>
    /// #synthese fonction qui permet d'ajuster le temps qui reste selon le niveau auquel le joueur est rendu
    /// </summary>
    public void AjusterTempsParNiveau()
    {
        _temps = _tempsIni + (30 * (_niveau - 1));
    }
}
