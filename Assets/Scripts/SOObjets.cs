using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp3 Classe qui permet de creer des assets qui contiendrons toutes les donnees de chaques objets de la boutique.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

[CreateAssetMenu(fileName = "Objet", menuName = "Objet boutique")]
public class SOObjet : ScriptableObject
{
    // definit toutes sortes de valeurs afin de determiner de quelle objet il s'agit qui seront fournie dans l'inspecteur
    [Header("LES DONNÉES")]
    [SerializeField] string _nom = "Double Saut";
    [SerializeField][Tooltip("icône de l'objet pour la boutique")] Sprite _sprite;
    [SerializeField][Range(0, 200)] int _prixDeBase = 30;
    [SerializeField, TextArea] string _description;
    [SerializeField][Tooltip("Cet objet donne-t-il droit au double saut?")] bool _donneDroitDoubleSaut = false;
    [SerializeField][Tooltip("Cet objet donne-t-il droit a une vitesse plus eleve?")] bool _donneDroitVitesseEleve = false;

    // rend public toutes ces valeurs afin que les objets puissent etre afficher dans les panneauobjet de la boutique
    public string nom { get => _nom; set => _nom = value; }
    public Sprite sprite { get => _sprite; set => _sprite = value; }
    public int prix
    { 
        get
        {
            int prix = Mathf.RoundToInt(_prixDeBase);
            return prix;
        }
    }
    public string description { get => _description; set => _description = value; }
    public bool donneDroitDoubleSaut { get => _donneDroitDoubleSaut; set => _donneDroitDoubleSaut = value; }
    public bool donneDroitVitesseEleve { get => _donneDroitVitesseEleve; set => _donneDroitVitesseEleve = value; }
}