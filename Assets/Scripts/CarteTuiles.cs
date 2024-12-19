using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Classe a ajouter sur les cartes des tuiles.
/// Permet de definir si une tuile sera présente selon des probabilités donner dans l'inspecteur,
/// une fois toutes les tuiles definies, envoie toutes les tuiles dans la tilemap de niveau
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>
public class CarteTuiles : MonoBehaviour
{
    // cree un slider qui peut etre changer dans l'inspecteur representant les probabilites qu'une tuile aparaisse
    [SerializeField, Range(0f, 1f)] float _probabilite;

    /// <summary>
    /// Fonction qui va chercher les components necessaire et qui donne la position de chaque tuile
    /// </summary>
    void Awake()
    {
        // va chercher la tilemap de chaque carte tuile
        Tilemap tilemap = GetComponent<Tilemap>();
        // definie les bounds de ces tilemaps
        BoundsInt bounds = tilemap.cellBounds;
        // va chercher le parent "niveau" des cartes tuiles
        Niveau niveau = GetComponentInParent<Niveau>();
        // definit le decalage necessaire au tuile pour qu'il aparaisse a la bonne place
        Vector3Int decalage = Vector3Int.FloorToInt(transform.position);

        // #synthese déplacement du tirage pour qu'il ne le fait qu'une fois par carte tuiles et non a chaque tuiles
        // definie un nombre aleatoire pour voir si la carte des tuiles va pouvoir etre presente
        float nbAlea = Random.Range(0f,1f);

        //  a chaque tuiles dans les axes des y et des x...
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                // definie la position de cette tuile
                Vector3Int pos = new Vector3Int (x , y , 0);
                // appelle la fonction pour traiter les tuiles
                TraiterUneTuile(tilemap, niveau, decalage, pos, nbAlea);
            }
        }
        // desactive les tuiles des cartes tuiles car elles ne sont plus utiles
        gameObject.SetActive(false);
    }

    // #synthese ajout des parametres tilemap, niveau, decalage, pos et nbAlea dans le summary de la fonction
    /// <summary>
    /// Fonction qui traite chaque tuile, si elle son presente ou non, et les envoie dans la tilemap du niveau
    /// </summary>
    /// <param name="tilemap">Tilemap de la carte tuile a traiter</param>
    /// <param name="niveau">Script Niveau appartenant au parent des cartes tuiles, soit le niveau</param>
    /// <param name="decalage">Decalage a definir pour que les tuiles puissent apparaitrent aux bonnes places dans le niveau</param>
    /// <param name="pos">Position de la tuile a traiter</param>
    /// <param name="nbAlea">Nombre entre 0 et 1 qui represente le pourcentage d'apparition de cette carte</param>
    void TraiterUneTuile(Tilemap tilemap, Niveau niveau, Vector3Int decalage, Vector3Int pos, float nbAlea)
    {
        // va chercher la tuile dans la tilemap selon la position donnee
        TileBase tuile = tilemap.GetTile(pos);
        // si le nombre aleatoire est plus petit que la probabilite et que la tuile n'est pas null...
        if(nbAlea <= _probabilite && tuile != null)
        {
            // appelle la fonction CreerTuile dans la classe niveau afin de lui donner la tuile
            niveau.CreerTuile(decalage, pos, tuile);
        }
        // sinon, il n'y a pas de tuile a cette position
        else { tilemap.SetTile(pos, null); }
    }

    /// <summary>
    /// Fonction qui definit la transparance des tuiles selon leur probabilite detre presente dans le niveau
    /// </summary>
    void OnDrawGizmos()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.color = new Color(1, 1, 1, _probabilite);
    }
}
