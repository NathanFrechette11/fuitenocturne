using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

/// <summary>
/// #tp4 classe a ajouter au canvas de la scene de tableau des scores. permet de gerer toutes l'affichage
/// des donnees dans le tableau
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class TableauScores : MonoBehaviour
{
    // donne acces au scriptable object de la sauvegarde
    [SerializeField] SOSauvegarde _donneesSauvegarde;
    // donne acces au prefab des vignette de score
    [SerializeField] PanneauPointage _prefabPanneauPointage;
    // donne acces au groupe dans lequel instentier les vignettes
    [SerializeField] GameObject _groupePanneau;
    // donne acces au boutton qui ramenne le joueur a la scene menu
    [SerializeField] GameObject _bouttonMenu;

    // #synthese definit le champ de texte du titre de la scene
    [SerializeField] TextMeshProUGUI _texteTitre;
    // #synthese definit les différentes données que le script aura besoin (ceux du personnage, ceux de la difficulté, ceux du pointage)
    // au lieux d'aller les cherchers dans l'instance de niveau
    [SerializeField] SOPointage _donneesPointage;
    [SerializeField] SODifficulte _donneesDifficulte;
    [SerializeField] SOPerso _donneesPerso;


    // quand le scene de tableau des scores se lance... 
    void Awake() {

        // #synthese appelle la fonction VerifierDifficulte du SOSauvegarde afin d'avoir le bon fichier de données
        _donneesSauvegarde.VerifierDifficulte(_donneesDifficulte.modeDifficile);
        // #synthese si le mode de jeu est en mode difficile, le titre de la scene va changer pour inclure "en mode difficile"
        if(_donneesDifficulte.modeDifficile) _texteTitre.text = "Tableau des scores en mode difficile";
        // #synthese sinon il va rester le meme que d'habitude
        else _texteTitre.text = "Tableau des scores";

        // on desactive le boutton pour pas que le joueur se rende directe au menu
        _bouttonMenu.SetActive(false);
        // on abonne la fonction ActiverBoutton au unityEvent du scriptable object de la sauvegarde
        _donneesSauvegarde.activerBoutton.AddListener(ActiverBoutton);
        // appelle la fonction pour afficher les meilleurs scores qui sont en mémoire
        AfficherMeilleursScores();
    }

    /// <summary>
    /// fonction qui permet de determiner sil y a des donnees dans le fichier fournie,
    /// afin de pouvoir afficher les scores dans le tableau
    /// </summary>
    void AfficherMeilleursScores() {
        // Vérifier s'il y a un fichier de sauvegarde
        string cheminEtFichier = Application.persistentDataPath + "/" + _donneesSauvegarde.nomFichier;
        if (File.Exists(cheminEtFichier)) {
            // lit le contenu du fichier
            string contenu = File.ReadAllText(cheminEtFichier);
            // si il n'est pas vide...
            if(contenu != "")
            {
                // si ce ne sont pas des colonnes vide...
                if(contenu != "{}")
                {
                    // va lire les donnees du fichier
                    _donneesSauvegarde.LireFichier(false);
                }
                // sinon...
                else
                {
                    // creer des valeurs par default qui seront enregistrer dans le fichier apres
                    _donneesSauvegarde.CreerScoresParDefaut();
                    Debug.LogWarning("Fichier vide : " + cheminEtFichier);
                }
            }
            // sinon...
            else
            {
                // creer des valeurs par default qui seront enregistrer dans le fichier apres
                _donneesSauvegarde.CreerScoresParDefaut();
                Debug.LogWarning("Fichier vide : " + cheminEtFichier);
            }
        }
        else {
            // Charger les scores par défaut si le fichier de sauvegarde n'existe pas
            _donneesSauvegarde.CreerScoresParDefaut();
            Debug.LogWarning("Fichier inexistant : " + cheminEtFichier);
        }

        // s'il y a plus ou egal a 6 donnees dans la liste...
        if (_donneesSauvegarde.lesDonneesPointage.Count >= 6) {
            // efface les donnees a partir de la 5e donnee, jusqua la valeur total de la grandeur de la liste moins les 5 premieres donnees
            _donneesSauvegarde.lesDonneesPointage.RemoveRange(5, _donneesSauvegarde.lesDonneesPointage.Count - 5);
        }
        // cree une nouvelle donnee de score avec le joueur actuelle ("TOI" par default), qui va chercher le score du joueur
        string nomJoueur = "TOI";
        int pointageJoueur = _donneesPointage.pointage;
        _donneesSauvegarde.lesDonneesPointage.Add(new DonneesPointage{nom = nomJoueur, pointage = pointageJoueur, joueurActuel = true});

        // trie la liste des donnees en ordre decroissant afin d'afficher le meilleur score en premier
        _donneesSauvegarde.lesDonneesPointage.Sort((x, y) => y.pointage.CompareTo(x.pointage));


        // definit une variable qui va servir de compteur
        int i = 0;
        // definit un bool qui servira a savoir si le joueur a deja été classé
        bool joueurEstClasse = false;
        // pour toutes les donnees de type DonneesPointage present dans la liste des donnees...
        foreach(DonneesPointage donnees in _donneesSauvegarde.lesDonneesPointage)
        {
            // ajoute 1 au compteur
            i++;
            // on desactive le inputfield et le boutton de la vignette
            _prefabPanneauPointage.DesactiverInput();
            // on donne les valeurs de la place de la vignette, le nom du joueur de la vignette, et son en rajoutant des 0 a
            // gauche du score pour atteindre 6 chiffre a chaque vignette
            _prefabPanneauPointage.place = i;
            _prefabPanneauPointage.champNom.text = i + ". " + donnees.nom;
            _prefabPanneauPointage.champPointage.text = donnees.pointage.ToString().PadLeft(6, '0');
            // sil sagit du joueur actuel et quil nest pas classe 6e...
            if(donnees.joueurActuel && i !=6)
            {
                // le joueur a deja ete classe
                joueurEstClasse = true;
                // la vignette du joueur actuel change de fond pour se demarquer
                _prefabPanneauPointage.fond.color = new Color32(0xBA, 0x47, 0x00, 0xFF);
                // et on active le inputfield et le boutton
                _prefabPanneauPointage.ActiverInput();
            }
            // la vignette a la 6e place a un fond plus foncer pour dire qu'elle va bientot partir
            if(i==6) _prefabPanneauPointage.fond.color = new Color32(0x55, 0x2C, 0x13, 0x90);
            // on instentie les vignettes dans le groupe des vignettes de score
            Instantiate(_prefabPanneauPointage, _groupePanneau.transform);
            // et les vignettes retrouvent leur couleur de fond normal
            _prefabPanneauPointage.fond.color = new Color32(0x55, 0x2C, 0x13, 0xFF);
        }
        // si le joueur actuel na toujours pas ete classe, on appelle lirefichier en lui disant que cest la fin de l'affichage
        if(joueurEstClasse == false)_donneesSauvegarde.LireFichier(true);

    }

    /// <summary>
    /// fonction qui permet d'afficher le boutton qui va ammener le joueur a la scene du menu principale
    /// </summary>
    void ActiverBoutton()
    {
        Debug.Log("actif");
        _bouttonMenu.SetActive(true);
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        // #synthese remet aux valeurs initiales les données du perso
        _donneesPerso.Initialiser();
        _donneesPerso.InitialiserBonus();
        // #synthese remet les valeurs de pointages a zero
        _donneesPointage.Initialiser();

        // #synthese remmet la difficulter par defaut a normal
        _donneesDifficulte.modeDifficile = false;

    }
}
