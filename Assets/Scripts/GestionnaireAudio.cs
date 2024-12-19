using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp4 Classe a ajouter sur le gestionnaireAudio.
/// Elle permet au gestionnaire de definir les volumes auquel les pistes doivent etre jouees et de changer l'etat de lecture des pistes.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class GestionnaireAudio : MonoBehaviour
{
    // definie le Volume de la piste de son quand elle doit etre jouee et cree un accesseur pour y acceder ailleur.
    [SerializeField] float _volumeMusicalRef = 1f;
    public float volumeMusicalRef => _volumeMusicalRef;
    // definie un tableau de piste musicale et cree un accesseur pour y acceder ailleur.
    PisteMusicale[] _tPistes;
    public PisteMusicale[] tPistes => _tPistes;

    // cree une instance statique de la classe GestionnaireAudio et un accesseur pour y acceder ailleur.
    static GestionnaireAudio _instance;
    public static GestionnaireAudio instance => _instance;

    void Awake()
    {
        // appelle la fonction DevenirInstanceSingleton pour s'assurer qu'il n'y a qu'une seule instance de la classe GestionnaireAudio.
        if(DevenirInstanceSingleton() == false) return;
        // ne detruit pas l'objet lorsque la scene change.
        DontDestroyOnLoad(gameObject);
        // donne les enfants de l'objet qui on le script PisteMusicale a _tPistes.
        _tPistes = GetComponentsInChildren<PisteMusicale>();
    }

    /// <summary>
    /// fonction qui permet de changer letat de lecture d'une piste musicale selon si elle est active ou non.
    /// </summary>
    /// <param name="type">type de piste audio qui est doit etre fournie</param>
    /// <param name="estActif">bool qui determine si la piste va etre active ou non</param>
    public void ChangerEtatLecturePiste(TypePiste type, bool estActif)
    {
        // pour chaque piste dans le tableau de piste, si le type de la piste est le meme que le type fourni, alors on change l'etat de lecture de la piste.
        foreach (PisteMusicale piste in _tPistes)
        {
            if (piste.type == type) piste.estActif = estActif;
        }
    }

    /// <summary>
    /// fonction qui permet de definir un bool qui indique si le gestionnaire audio va devenir une instance singleton ou non.
    /// </summary>
    /// <returns>true si succes, false si echec</returns>
    bool DevenirInstanceSingleton()
    {
        // si il y a deja une instance de la classe GestionnaireAudio, alors on detruit l'objet et on retourne false.
        if (_instance != null)
        {
            // affiche un message d'erreur dans la console.
            Debug.LogWarning("Tentative de créer une deuxième instance de GestAudio. La deuxième instance sera détruite.");
            // detruit l'objet.
            Destroy(gameObject);
            // retourne false.
            return false; //échec!
        }
        // sinon, on definit l'instance de la classe GestionnaireAudio a l'objet courant et on retourne true.s
        _instance = this;
        return true; //succès!
    }
}