using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe qui est directement lié a la classe du personnage par héritage.
/// Elle permet au personnage de détecter s'il touche au sol ou non, et dessine un Gizmos pour nous donner un visuel
/// Auteurs du code: Nathan Fréchette et Cynthia Bélanger
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class DetecteurSol : MonoBehaviour
{
    // #synthese changement de nom de variable (avant était coll, maintenant est _coll)
    // collider du personnage, est définie dans le Start
    Collider2D _coll;

    // vecteur qui défini la taille de la boite qui verifie le sol
    [SerializeField] Vector2 _tailleBoite = new Vector2(0.55f,0.22f);

    // #synthese changement de nom de variable (avant était boxCenter(je pense), maintenant est _centreBoite)
    // vecteur qui défini le centre de la boite qui verifie le sol
    [SerializeField] Vector2 _centreBoite;

    // nous permet de définir dans l'inspecteur sur quel layer on veut la detection du sol
    [SerializeField] LayerMask _layerMask;
    // booléen qui definit si le personnage est en train de toucher le sol
    protected bool _estAuSol = false;

    void Start()
    {
        // va chercher la composante Collider2D
        _coll = GetComponent<Collider2D>();
    }
    virtual protected void FixedUpdate()
    {
        // appelle la fonction qui permet de vérifier le sol
        VerifierSol();
    }

    /// <summary>
    /// Permet de définir la valeur du centre de la boite qui vérifie le sol.
    /// Une fois le centre défini, la boite est créer pour rester un peu en dessous du collider du perso.
    /// Donne la valeur du collider du perso a _estAuSol (detecte seulement les collisions avec le layer definie dans l'inspecteur, donc le sol)
    /// </summary>
    void VerifierSol()
    {
        // #synthese changement des noms des variables comme mentionner plus haut
        _centreBoite = transform.position + Vector3.down * 0.9f;
        _coll = Physics2D.OverlapBox(_centreBoite, _tailleBoite, 0, _layerMask);
        _estAuSol = _coll != null;
    }

    /// <summary>
    /// Permet d'avoir le visuel de la boite que le jeu marche ou non.
    /// Change la couleur selon si le perso touche le sol ou non.
    /// </summary>
    void OnDrawGizmos()
    {
        if(Application.isPlaying == false) VerifierSol();
        Gizmos.color = _estAuSol ? Color.green : Color.red;

        // #synthese ajout de cette ligne afin que le gizmos suit le perso correctement, pas qu'il ai un petit decalage quand le perso bouge
        _centreBoite = transform.position + Vector3.down * 0.9f;
        // #synthese changement des noms des variables comme mentionner plus haut
        Gizmos.DrawWireCube(_centreBoite, _tailleBoite);
    }
}