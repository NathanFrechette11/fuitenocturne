using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// #tp4 Classe a ajouter sur le canvas de linterface a afficher pendant la partie.
/// Elle permet de mettre a jour les informations affichees a l'ecran et de gerer le timer de la partie.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class InterfaceJeu : MonoBehaviour
{
    // #synthese definie si les points bonus ont deja ete ajouter au score du joueur
    bool _pointBonusDejaAjoute;

    // definie si les bonus de double saut et de vitesse ont ete affiches.
    bool _bonusDoubleSautAffiche;
    bool _bonusVitesseAffiche;

    // #synthese ajout des commentaires individuelles a chaque déclarations de variables
    // definie les champs de texte pour afficher les informations du joueur.
    [Header("LES DONNÉES")]
    // definie le champ de texte qui va afficher la vie restante du joueur
    [SerializeField] TextMeshProUGUI _champVie;
    // definie le champ de texte qui va afficher le temps restant au jeu
    [SerializeField] TextMeshProUGUI _champTemps;
    // definie le champ de texte qui va afficher le niveau actuel
    [SerializeField] TextMeshProUGUI _champNiveau;
    // definie le champ de texte qui va afficher le nombre de points accumules par le joueur
    [SerializeField] TextMeshProUGUI _champPointage;
    // definie le champ de texte qui va afficher le nombre de billets que le joueur a en tout
    [SerializeField] TextMeshProUGUI _champArgent;

    [Header("points bonus")]
    // definie le champ de texte qui va afficher le nombre de temps qui reste * le nombre de point fournie
    [SerializeField] TextMeshProUGUI _champBonusTemps;
    // definie le champ de texte qui va afficher le pointage total pour le bonus de temps restant
    [SerializeField] TextMeshProUGUI _champPointsTemps;

    // #synthese definie le champ de texte qui va afficher le texte de la zone des points bonus pour le nombre de bonus restant sur la scene
    [SerializeField] TextMeshProUGUI _champTexteBonusBonus;

    // definie le champ de texte qui va afficher le nombre de bonus qui reste sur la scene * le nombre de point fournie
    [SerializeField] TextMeshProUGUI _champBonusBonus;
    // definie le champ de texte qui va afficher le pointage total pour le bonus de bonus restant
    [SerializeField] TextMeshProUGUI _champPointsBonus;

    // #synthese definie le champ de texte qui va afficher le texte de la zone des points bonus pour le nombre d'ennemis tues
    [SerializeField] TextMeshProUGUI _champTexteBonusEnnemis;

    // definie le champ de texte qui va afficher le nombre d'ennemis tues * le nombre de point fournie
    [SerializeField] TextMeshProUGUI _champBonusEnnemis;
    // definie le champ de texte qui va afficher le pointage total pour le bonus d'ennemis tues
    [SerializeField] TextMeshProUGUI _champPointsEnnemis;
    // definie le champ de texte qui va afficher le nombre de points bonus total qui sera ajouter au score
    [SerializeField] TextMeshProUGUI _champPointsBonusTotal;
    // definie le prefab du tableau des points bonus qui seront fournit a la fin du niveau
    [SerializeField] GameObject _zonePointsBonus;
    // definie le bouton qui permettra de ce rendre a la boutique a la fin du niveaus
    [SerializeField] GameObject _bouton;


    // Start is called before the first frame update
    void Start()
    {
        // #synthese definit que le joueur na pas deja recu les points bonus a son score
        _pointBonusDejaAjoute = false;

        // lance la coroutine pour le timer de la partie.
        StartCoroutine(CoroutineMinuteur());
        // determine que les bonus n'ont pas ete affiches.
        _bonusDoubleSautAffiche = false;
        _bonusVitesseAffiche = false;
        // met a jour l'affichage des informations du joueur.
        MiseAJourAffichage();
        // ajoute a l'unityEvent mise a jour de SOPerso et SOPointage les fonctions a appeler lors de la mise a jour des donnees du joueur.
        Niveau.instance.donneesPointage.evenementMiseAJour.AddListener(MiseAJourAffichage);
        Niveau.instance.donneesPerso.evenementMiseAJour.AddListener(MiseAJourAffichage);
        // desactive la zone de points bonus et le bouton de fin de niveau.
        _zonePointsBonus.SetActive(false);
        _bouton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // si le niveau est fini, alors on affiche la zone de points bonus et on affiche le pointage.
        if(Niveau.instance.finNiveau == true)
        {
            _zonePointsBonus.SetActive(true);
            AfficherPointage();
        }
        // si le bouton de fin de niveau devrait etre actif, alors on l'affiche.
        if (Niveau.instance.activerBouton == true) _bouton.SetActive(true);
    }

    /// <summary>
    /// fonction qui calcul et qui affiche les points bonus obtenus a la fin du niveau.
    /// </summary>
    void AfficherPointage()
    {
        // #synthese ternaire qui verifie si le nombre d'ennemis tues et si les bonus restant sont plus grands que 1 et affiche le texte au singulier ou au pluriel selon le resultat
        _champTexteBonusBonus.text = "bonus restant" + ((Niveau.instance.nbPillulesSurScene>=2)?"s":"") + " : ";
        _champTexteBonusEnnemis.text = "ennemi" + ((Niveau.instance.ennemisTue>=2)?"s":"") + " tué" + ((Niveau.instance.ennemisTue>=2)?"s":"") + " :";

        // calcul des points bonus obtenus a la fin du niveau.
        int pointsTemps = Niveau.instance.donneesPerso.temps * 10;
        int pointsBonus = Niveau.instance.nbPillulesSurScene * 60;
        int pointsEnnemis = Niveau.instance.ennemisTue * 100;
        // affichage des points bonus obtenus a la fin du niveau ainsi que le total des points bonus.
        _champBonusTemps.text = Niveau.instance.donneesPerso.temps + "s * 10";
        _champPointsTemps.text = "+ " + pointsTemps;
        _champBonusBonus.text = Niveau.instance.nbPillulesSurScene + " * 60";
        _champPointsBonus.text = "+ " + pointsBonus;
        _champBonusEnnemis.text = Niveau.instance.ennemisTue + " * 100";
        _champPointsEnnemis.text = "+ " + pointsEnnemis;
        _champPointsBonusTotal.text = pointsTemps + pointsBonus + pointsEnnemis + "";

        // #synthese si les points bonus n'ont pas deja ete donner au joueur, donne les points bonus au joueur et definit que les points ont deja ete donnee
        if(_pointBonusDejaAjoute == false)
        {
            Niveau.instance.donneesPointage.pointage += pointsTemps + pointsBonus + pointsEnnemis;
            _pointBonusDejaAjoute = true;
        }
    }

    /// <summary>
    /// fonction qui met a jour l'affichage des informations du joueur a chaque fois que les donnees du joueur sont mises a jour.
    /// </summary>
    void MiseAJourAffichage()
    {
        // met a jour les informations du joueur, fait en sorte que le pointage soit affiche sur 6 chiffres en ajoutant des 0 a gauche.
        _champPointage.text = Niveau.instance.donneesPointage.pointage.ToString().PadLeft(6, '0');
        _champVie.text = Niveau.instance.donneesPerso.vie + "";
        _champNiveau.text = "Niveau " + Niveau.instance.donneesPerso.niveau;
        _champArgent.text = Niveau.instance.donneesPerso.argent + "";

        // si le joueur a acheter un bonus de vitesse ou de double saut et qu'ils n'etaient pas deja affiche, alors on affiche le bonus correspondant.
        if(Niveau.instance.donneesPerso.vitesseEleve && _bonusVitesseAffiche == false)
        {
            Niveau.instance.panneauVignette.AfficherInventaire(Niveau.instance.bonusVitesse);
            _bonusVitesseAffiche = true;
        }
        if(Niveau.instance.donneesPerso.doubleSaut && _bonusDoubleSautAffiche == false)
        {
            Niveau.instance.panneauVignette.AfficherInventaire(Niveau.instance.bonusDoubleSaut);
            _bonusDoubleSautAffiche = true;
        }
        // si le joueur n'a plus de vie, alors on change de scene pour afficher les scores.
        if(Niveau.instance.donneesPerso.vie == 0) Niveau.instance.navigation.AllerSceneScores();
    }

    /// <summary>
    /// coroutine qui gere le timer de la partie.
    /// </summary>
    /// <returns>attends pendant 1 seconde</returns>
    IEnumerator CoroutineMinuteur()
    {
        // tant que le temps restant est superieur ou egal a 0 et que le niveau n'est pas fini, alors on met a jour le texte du timer avec le temps restant et
        //on diminue le temps restant.
        while (Niveau.instance.donneesPerso.temps >= 0 && Niveau.instance.finNiveau == false)
        {
            // Mettre à jour le texte du timer avec le temps restant
            _champTemps.text = Niveau.instance.donneesPerso.temps + "s";
            // Attendre une seconde
            yield return new WaitForSeconds(1f);
            // Diminuer le temps restant
            if(Niveau.instance.finNiveau == false) Niveau.instance.donneesPerso.temps -= 1;

            // #synthese si le timer est rendu a 0, envoit le joueur au tableau des scores (fin de la partie)
            if(Niveau.instance.donneesPerso.temps == 0) Niveau.instance.donneesPerso.vie = 0;
        }
    }
}
