using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe a ajouter sur les prefabs des salle.
/// Elle permet de definir la taille des salles ainci que de dessiner des gizmos selon leur taille.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>
public class Salle : MonoBehaviour
{
    // #tp3 definit tout les reperes dans les salles ou les items vont pouvoir apparaitre
    [SerializeField] List<Transform> _lesReperes = new List<Transform>();
    [SerializeField] Transform _repereEffector;
    [SerializeField] Transform _reperePerso;

    // #synthese definit les ennemies placer dans chacune des salles
    [SerializeField] GameObject _ennemie;

    // #tp4 permet de recuperer le spriteRenderer du fond des salles dans l'inspecteur
    [SerializeField] SpriteRenderer _fond;

    // #tp3 fournie une reference pour l'instance du personnage avec son getter afin que les champs de texte apparaisent la ou le perso est
    GameObject _persoInstance;
    public GameObject persoInstance => _persoInstance;

    // #tp3 bool qui permet de definir si la salle a deja la cle ou la porte, et renvoie la valeur du bool
    bool _contientClePorte = false;
    public bool contientClePorte
    {
        get { return _contientClePorte; }
        set { _contientClePorte = value; }
    }

    // varible qui appartient au script qui permet de definir la taille de chaque salle,
    // avec son getteur pour permettre a d'autre script d'y avoir acces en lecture seulement
    static private Vector2Int _taille = new Vector2Int(32,18);
    static public Vector2Int taille => _taille;
    

    // #synthese ajout parametre modele et de ce que la fonction retourne dans le summary de la fonction
    ///  <summary>
    /// #tp3 fonction qui permet de placer la porte, la cle, l'effector et l'activateur sur des reperes dans la salle
    /// </summary>
    /// <param name="modele">modele de l'ojet a fournir qui devrait etre instancier</param>
    /// <returns>retourne la position que cet objet va occuper</returns>
    public Vector2Int PlacerSurRepere(GameObject modele)
    {
        // #tp3 va chercher un nombre aleatoire selon le nombre de repere present dans la salle
        int repere = Random.Range(0, _lesReperes.Count);
        // #tp3 prend la position du repere qui se situe a la position aleatoire dans la liste des reperes
        Vector3 pos = _lesReperes[repere].position;
        // #tp3 si l'objet a instantier est l'effector...
        if(modele.name == "effector")
        {
            // #tp3 il se fait instantier a la position du repereEffector qui se trouve dans les salles
            Instantiate(modele , _repereEffector.position , Quaternion.identity , transform.parent);
            // #tp3 et renvoie sa position pour qu'elle soit retirer des positions libres
            return Vector2Int.FloorToInt(_repereEffector.position);
        }

        // #tp3 sinon...
        else
        {
            // #tp3 on instantie le modele de lobjet qui a été envoyer
            Instantiate(modele , pos , Quaternion.identity , transform.parent);
            // #tp3 et on retourne sa position pour lenlever de la liste des pos libres
            return Vector2Int.FloorToInt(pos);
        }
    }

    /// <summary>
    /// #synthese fonction qui permet de faire afficher les ennemies dans les salles ou de les laisser désactivé 
    /// </summary>
    /// <param name="vaApparaitre">"true" ou "false" a fournir qui sera envoyé dans le SetActive des ennemies</param>
    public void FaireApparaitreEnnemies(bool vaApparaitre)
    {
        if(vaApparaitre) _ennemie.SetActive(true);
        else _ennemie.SetActive(false);
    }

    // #synthese ajout du parametre perso et de ce que la fonction retourne dans le summary de la fonction
    /// <summary>
    /// #tp3 fonction qui permet de placer le personnage sur la scene
    /// </summary>
    /// <param name="perso">modele du personnage a fournir afin qu'il puisse etre instancier</param>
    /// /// <returns>retourne la position a laquelle le personnage apparaitra</returns>
    public Vector2Int PlacerPerso(GameObject perso)
    {
        // #tp3 va chercher la position du repereperso qui se trouve dans les salles
        Vector3 pos = _reperePerso.position;

        // #tp3 instantie le perso a cette position
        _persoInstance = Instantiate(perso , pos , Quaternion.identity , transform.parent.parent);

        // tp4 envoie le transform du perso a la camera cinemachine afin qu'elle le suit
        Niveau.instance.cam.Follow = _persoInstance.transform;

        // #tp3 retourne la position afin que celle ci soit retirer de la liste des positions libres
        return Vector2Int.FloorToInt(pos);
    }


    /// <summary>
    /// fonction qui dessine un gizmos selon la taille de la salle pour nous permettre de voir
    /// les dimensions de chaques salles une fois qu'elles seront l'une a coté de l'autre
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3 ( _taille.x , _taille.y , 0));
    }


    /// <summary>
    /// #tp4 fonction qui permet de changer le fond des salles tout en lui donnant la taille exact de chaque salle
    /// </summary>
    /// <param name="fond">Sprite avec lequel on veut changer le fond des salles</param>
    public void ChangerFond(Sprite fond)
    {
        _fond = GetComponentInChildren<SpriteRenderer>();
        _fond.sprite = fond;
        _fond.transform.localScale = new Vector2(1.05f,0.9f);
    }
}
