using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Events;
using System.Runtime.InteropServices;

/// <summary>
/// #tp4 scriptableobject des donnees de sauvegarde, definit le chemin a prendre pour envoyer les donnees de sauvegardes
/// ainsi que dans quel fichier les mettre. Gère la sauvegarde ainsi que la lecture des donnees.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

// creer un menuasset afin de pouvoir creer un sosauvegarde
[CreateAssetMenu(menuName = "TIM/Sauvegarde", fileName = "Sauvegarde")]
public class SOSauvegarde : ScriptableObject {

    // permet de lire le script JSON en webGL a l'aide du plugin fournit par le prof
    [DllImport("__Internal")] 
    static extern void SynchroniserWebGL();

    // definit un unityevent qui permet d'activer le bouton qui ammene au menu une fois lancer et cree un accesseur afin de l'utiliser ailleur
    UnityEvent _activerBoutton = new UnityEvent();
    public UnityEvent activerBoutton => _activerBoutton;
    // definit le nom que le fichier doit avoir afin de pouvoir y acceder et cree un accesseur afin de l'utiliser ailleur
    [SerializeField] string _nomFichier = "donnees.tim";
    public string nomFichier => _nomFichier;

    // #synthese definit le fichier des scores pour quand le jeu est en mode normal
    [SerializeField] string _nomFichierNormal = "donnees.tim";
    // #synthese definit le fichier des scores pour quand le jeu est en mode difficile
    [SerializeField] string _nomFichierDifficile = "donnees.tim";

    // definit une liste avec le type de donnees creer par la classe DonneesPointage et cree un accesseur afin de l'utiliser ailleur
    [SerializeField] List<DonneesPointage> _lesDonneesPointage = new List<DonneesPointage>();
    public List<DonneesPointage> lesDonneesPointage => _lesDonneesPointage;


    /// <summary>
    /// #synthese fonction qui permet de verifier quel fichier de sauvegarde il faut prendre selon le mode de jeu
    /// </summary>
    /// <param name="modeDifficile">"true" ou "false" a fournir qui permet de definir si le jeu est en mode difficile ou non</param>
    public void VerifierDifficulte(bool modeDifficile)
    {
        if(modeDifficile)
        {
            _nomFichier = _nomFichierDifficile;
        }
        else
        {
            _nomFichier = _nomFichierNormal;
        }
    }

    /// <summary>
    /// Fonction qui permet de creer des valeurs par defaut si jamais il ny a pas de donnees disponibles
    /// </summary>
    public void CreerScoresParDefaut() {
        // Ajouter ici les scores par défaut
        _lesDonneesPointage.Clear();
        _lesDonneesPointage.Add(new DonneesPointage { nom = "Joueur1", pointage = 1000 , joueurActuel = false});
        _lesDonneesPointage.Add(new DonneesPointage { nom = "Joueur2", pointage = 900 , joueurActuel = false});
        _lesDonneesPointage.Add(new DonneesPointage { nom = "Joueur3", pointage = 800 , joueurActuel = false});
        _lesDonneesPointage.Add(new DonneesPointage { nom = "Joueur4", pointage = 700 , joueurActuel = false});
        _lesDonneesPointage.Add(new DonneesPointage { nom = "Joueur5", pointage = 600 , joueurActuel = false});

        // Enregistrer les donnees
        EnregistrerSauvegarde();
    }

    /// <summary>
    /// fonction qui permet de changer le nom de la donnee du joueur actuel par le nom que le joueur a entrer
    /// dans le inputfield de la vignette des scores
    /// </summary>
    /// <param name="texte">inputfield de la vignette des scores a fournir</param>
    public void ChangerNom(TMP_InputField texte)
    {
        // pour toutes les donnees de type DonneesPointage present dans la liste des donnees...
        foreach(DonneesPointage donnees in _lesDonneesPointage)
        {
            // si la donnee appartient au joueur actuel...
            if(donnees.joueurActuel)
            {
                // au change le nom de cette donnee par celui que le joueur a entre
                donnees.nom = texte.text;
                // on enregistre la modification
                EnregistrerSauvegarde();
                // et on recharge les donnees avec les nouvelles
                LireFichier(false);
            }

        }
    }

    /// <summary>
    /// fonction qui permet d'afficher le nouveau nom que le joueur a entre dans le tableau des scores a l'aide
    /// de la vignette de score du joueur actuel fournie quand on appelle la fonction
    /// </summary>
    /// <param name="panneau">panneau des scores du joueur actuel a fournir</param>
    public void AfficherNouveauNom(PanneauPointage panneau)
    {
        // pour toutes les donnees de type DonneesPointage present dans la liste des donnees...
        foreach(DonneesPointage donnees in _lesDonneesPointage)
        {
            // sil sagit du joueur actuel...
            if(donnees.joueurActuel)
            {
                // on change le nom dans le panneau en lui dennant la place du panneau dans le tableau ainsi que le nom entre par le joueur
                panneau.champNom.text = panneau.place + ". " +donnees.nom;
                // on remet la couleur de la vignette a la couleur normal
                panneau.fond.color = new Color32(0x55, 0x2C, 0x13, 0xFF);
                // on definit que la vignette n'appartient plus au joueur actuel
                donnees.joueurActuel = false;
                // et on fait apparaitre le boutton qui ammene vers le menu
                _activerBoutton.Invoke();
            }

        }

        //#synthese changement de place de ces lignes de code pour que la sauvegarde marche correctement (if... et EnregistrerSauvegarde())
        // s'il y a plus ou egal a 6 donnees dans la liste...
        if (_lesDonneesPointage.Count >= 6) {
            // efface les donnees a partir de la 5e donnee, jusqua la valeur total de la grandeur de la liste moins les 5 premieres donnees
            _lesDonneesPointage.RemoveRange(5, _lesDonneesPointage.Count - 5);
        }
        // on sauvegarde encore une fois les donnees puisquils ont ete change a nouveau
        EnregistrerSauvegarde();
    }

    /// <summary>
    /// fonction qui permet de sauvegarder les donnees qui sont dans la liste des donnees dans le dossier qui a ete fourni
    /// plus haut dans la classe
    /// </summary>
    public void EnregistrerSauvegarde() {
        // on cree une nouvelle liste a partir dune classe intermediaire
        ListeDonnees listeDonnees = new ListeDonnees();
        // on lui donne les donnees presente dans la liste des donnees
        listeDonnees.donnees = _lesDonneesPointage;
        // on transforme cette nouvelle liste en texte json
        string contenu = JsonUtility.ToJson(listeDonnees);
        // va chercher le chemin ainsi que le fichier ou va etre enregistrer les donnees
        string cheminEtFichier = Application.persistentDataPath + "/" + _nomFichier;
        // ecrit les donnees en format JSON dans le fichier donnee
        File.WriteAllText(cheminEtFichier, contenu);
        
        // si la platform utilise est webgl...
        if(Application.platform == RuntimePlatform.WebGLPlayer) 
        {
            // lance la synchronisation des donnees dans la page web
            SynchroniserWebGL();
            Debug.Log("Coucou WebGL");
        }
    }

    /// <summary>
    /// fonction qui permet de lire les donnees qui ont ete enregistrer dans le fichier
    /// fournit plus haut dans la classe
    /// </summary>
    /// <param name="fin">bool qui permet de savoir si on veut lire le fichier quand l'affichage des scores
    /// est completement terminer</param>
    public void LireFichier(bool fin) {
        // va chercher le chemin ainsi que le fichier ou va etre enregistrer les donnees
        string cheminEtFichier = Application.persistentDataPath + "/" + _nomFichier;
        Debug.Log(cheminEtFichier);
        
        // si le fichier existe bel et bien la ou le chemin indique...
        if (File.Exists(cheminEtFichier)) {
            // on envoit toutes les donnees du fichier dans la variable contenu
            string contenu = File.ReadAllText(cheminEtFichier);

            // Désérialiser le contenu JSON dans une liste de données de pointage
            ListeDonnees listeDonnees = JsonUtility.FromJson<ListeDonnees>(contenu);
            // donne les donnees dans la liste des donnees du SOSauvegarde
            _lesDonneesPointage = listeDonnees.donnees;
            // pour toutes les donnees de type DonneesPointage present dans la liste des donnees...
            foreach (DonneesPointage donnee in _lesDonneesPointage) {
                // si la variable fin est true, permet d'afficher le boutton qui ammene au menu
                if(fin) _activerBoutton.Invoke();
            }
            // permet la sauvegarde des donnees change en prog quand on est en mode edition
            #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }

    }
}







