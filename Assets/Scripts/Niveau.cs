using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UIElements;
using Cinemachine;

/// <summary>
/// Classe a ajouter sur le niveau.
/// Elle permet au niveau d'instancier les salles dans lui-meme ainsi que de creer une bordure
/// au extremité du niveau ou il n'y a pas de salle a instancier.
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>
public class Niveau : MonoBehaviour
{
    // #tp3 permet de definir la classe niveau en tant que singleton pour qu'il soit appeler de n'importe ou
    static Niveau _instance;
    static public Niveau instance => _instance;
    // #tp3 creation des evenements activation pour les bonus de niveau, et deverrouiller pour la porte
    UnityEvent _activation = new UnityEvent();
    public UnityEvent activation => _activation;
    UnityEvent _deverrouiller = new UnityEvent();
    public UnityEvent deverrouiller => _deverrouiller;
    // #tp3 creation des listes des positions libres et des positions ou il y a un repere
    [SerializeField] List<Vector2Int> _lesPosLibres = new List<Vector2Int>();
    [SerializeField] List<Vector2Int> _lesPosSurRepere = new List<Vector2Int>();
    // #tp3 variable pour definir tout les objets que le niveau devra instancier
    // #tp3 variable pour definir le prefab de la cle
    [SerializeField] GameObject _cle;
    // #tp3 variable pour definir le prefab de la porte
    [SerializeField] GameObject _porte;
    // #tp3 variable pour definir le prefab de l'activateur
    [SerializeField] GameObject _activateur;
    // #tp3 variable pour definir le prefab de la pillule
    [SerializeField] GameObject _pillule;
    // #tp3 variable pour definir le prefab de l'argent
    [SerializeField] GameObject _argent;
    // #tp3 variable pour definir le prefab de l'effector
    [SerializeField] GameObject _effector;
    // #tp3 variable pour definir le prefab du perso
    [SerializeField] GameObject _perso;
    public GameObject perso => _perso;

    // #tp3 variable pour definir le prefab de la retroaction
    [SerializeField] Retroaction _modeleRetroaction;
    Salle sallePerso;

    // #tp4 variables qui permettent d'acceder aux differents objets pour l'interface de jeu de la scene
    [SerializeField] GameObject _zoneInventaire;
    public GameObject zoneInventaire => _zoneInventaire;
    // #tp4 variable qui permet de definir le prefab panneauvignette pour afficher l'inventaire du joueur dans l'interface et cree un getteur pour y acceder ailleur
    [SerializeField] PanneauVignette _panneauVignette;
    public PanneauVignette panneauVignette => _panneauVignette;
    // #tp4 variable qui permet de definir l'image du bonus de vitesse pour l'inventaire du joueur
    [SerializeField] Sprite _bonusVitesse;
    public Sprite bonusVitesse => _bonusVitesse;
    // #tp4 variable qui permet de definir l'image du bonus de double saut pour l'inventaire du joueur
    [SerializeField] Sprite _bonusDoubleSaut;
    public Sprite bonusDoubleSaut => _bonusDoubleSaut;
    // #tp4 variables qui permettent d'acceder aux differents fonds des salles
    [SerializeField] Sprite _fondBas;
    [SerializeField] Sprite _fondHaut;
    
    // #tp4 variables qui permettent de definir le nombre d'ennemis tue et cree un accesseur pour y acceder ailleur
    int _ennemisTue = 0;
    public int ennemisTue
    {
        get => _ennemisTue;
        set
        {
            _ennemisTue = Mathf.Clamp(value, 0, int.MaxValue);
        } 
    }

    // #synthese definit le nombre de salles que le niveau va avoir et crée un getteur/setter afin d'y acceder ailleurs
    int _nbSalles;
    public int nbSalles => _nbSalles;

    // #synthese definit le nombre d'ennemies que le niveau devrait avoir et crée un getteur/setter afin d'y acceder ailleurs
    int _nbEnnemis;
    public int nbEnnemis
    {
        get => _nbEnnemis;
        set
        {
            _nbEnnemis = Mathf.Clamp(value, 0, int.MaxValue);
        } 
    }

    // #synthese definit les donnees pour le mode de jeu (donnees de difficulte)
    [SerializeField] SODifficulte _donneesDifficulte;
    public SODifficulte donneesDifficulte => _donneesDifficulte;

    // #synthese variable qui definit le nombre de billets qui vont etre instantier sur la scene
    // et crée un getteur/setteur afin d'y acceder ailleurs
    int _probabiliteArgent;
    public int probabiliteArgent
    {
        get => _probabiliteArgent;
        set
        {
            _probabiliteArgent = Mathf.Clamp(value, 0 ,int.MaxValue);
        }
    }
    // #synthese variable qui definit le nombre de pillule qui vont etre instantier sur la scene
    // et crée un getteur/setteur afin d'y acceder ailleurs
    int _probabiliteBonus;
    public int probabiliteBonus
    {
        get => _probabiliteBonus;
        set
        {
            _probabiliteBonus = Mathf.Clamp(value, 0 ,int.MaxValue);
        }
    }

    // #tp4 definit une variable qui permettent de definir si le niveau est fini et cree un getter/setter pour y acceder ailleurs
    bool _finNiveau;
    public bool finNiveau
    {
        get => _finNiveau;
        set
        {
            _finNiveau = value;
        }
    }
    // #tp4  definit une variable qui permettent de definir si le bouton de fin de niveau est actif
    // et cree un getter/setter pour y acceder ailleurs
    bool _activerBouton;
    public bool activerBouton
    {
        get => _activerBouton;
        set
        {
            _activerBouton = value;
        }
    }
    // #tp4 variable qui permet de definir le nombre de pillules sur la scene et cree un getter/setter pour y acceder ailleur
    int _nbPillulesSurScene = 0;
    public int nbPillulesSurScene
    {
        get => _nbPillulesSurScene;
        set
        {
            _nbPillulesSurScene = value;
        }
    }
    // #tp4 variables qui permettent de definir les sons du jeu et des booleens pour verifier si ils ont deja ete jouer
    // #tp4 variables qui permettent de definir le son de saut
    [SerializeField] AudioClip _sonSaut;
    // #tp4 variables qui permettent de definir le son d'atterrisage
    [SerializeField] AudioClip _sonAtterissage;
    // #tp4 variables qui permettent de definir le son de la porte
    [SerializeField] AudioClip _sonPorte;
    // #tp4 variables qui permettent de definir le la cle
    [SerializeField] AudioClip _sonCle;
    // #tp4 variables qui permettent de definir l'audio source du niveau
    AudioSource _audio;
    // #tp4 variables qui permettent de definir si le son d'atterisage a deja jouer et cree des getter/setter afin d'y acceder ailleur
    bool _sonSolDejaJouer;
    public bool sonSolDejaJouer
    {
        get => _sonSolDejaJouer;
        set
        {
            _sonSolDejaJouer = value;
        }
    }
    // #tp4 variables qui permettent de definir si le son de saut a deja jouer et cree des getter/setter afin d'y acceder ailleur
    bool _sonSautDejaJouer;
    public bool sonSautDejaJouer
    {
        get => _sonSautDejaJouer;
        set
        {
            _sonSautDejaJouer = value;
        }
    }

    // #tp3 bool pour verifier si l'activateur a deja été placé dans une salle
    bool _activateurDejaPresent = false;

    // #tp3 variables qui donnes acces aux differentes donnees que le niveau aura besoin, ainsi que les getters pour les autres scripts
    [SerializeField] SOPerso _donneesPerso;
    public SOPerso donneesPerso =>_donneesPerso;
    [SerializeField] SOPointage _donneesPointage;
    public SOPointage donneesPointage =>_donneesPointage;
    [SerializeField] SONavigation _navigation;
    public SONavigation navigation =>_navigation;

    // variable définissant la taille du niveau, autrement dit le nombre de salles qu'il y aura sur chaque colonnes et étages
    Vector2Int _taille = new Vector2Int(2,2);

    // tableau des prefabs des salles que j'ai fait, salles fournies dans l'inspecteur
    [SerializeField] Salle[] _tSallesModeles = new Salle[2];
    // variable qui donne nous permet d'acceder a la tilemap du niveau,
    // cree ensuite un acsesseur pour la tilemap afin que les autres scripts puissent y envoyer leurs tuiles
    [SerializeField] Tilemap _tilemap;
    public Tilemap tilemap => _tilemap;
    // nous permet de fournir une tuile de base pour contourner les extremités du niveau ou il n'y aura pas de salle
    [SerializeField] TileBase _tuileMurs;

    // #tp4 variables qui permettent de definir la camera virtuelle du niveau et un accesseur pour y acceder ailleur
    [SerializeField] CinemachineVirtualCamera _cam;
    public CinemachineVirtualCamera cam => _cam;

    /// <summary> 
    /// Fonction qui permet d'instancier une salle aléatoire selon le tableau des salles fournies,
    /// une fois les salles instancier, cree une bordure autour du niveau
    /// </summary>
    void Awake()
    {
        // #synthese remet le nombre d'ennemies que le joueur a tuée a 0 au debut de la partie
        _ennemisTue = 0;

        // va chercher la taille d'une salle et lui enleve 1 sur l'axe des x et y pour avoir la taille sans le plafond ni le mur de droite
        Vector2Int tailleAvecUneBordure = Salle.taille - Vector2Int.one;

        // #tp4 initialise les variables de fin de niveau et d'activation du bouton a false par defaut
        _finNiveau = false;
        _activerBouton = false;
        // #tp4 va chercher le composant audio source du niveau
        _audio = GetComponent<AudioSource>();
        // #tp4 met le sonSautDejaJouer a false par defaut
        _sonSautDejaJouer = false;
        
        // #tp3 verifie sil y a deja une instance de niveau sur la scene. si oui, detruit celui qui vient de creer, sinon en creer un
        if(_instance != null) {Destroy(gameObject); return;}
        _instance = this;
        
        // #synthese appelle la fonction dans le SOPerso qui permet d'ajuster le temps restant selon le nombre de niveau
        _donneesPerso.AjusterTempsParNiveau();
        // #synthese appelle la fonction qui fait la creation des niveaux en lui envoyant la taille que le niveau devrait avoir
        CreerSalles(tailleAvecUneBordure);
        // #synthese appelle la fonction qui place les tuiles aux contours du niveau en lui envoyant la taille que le niveau devrait avoir
        FermerNiveau(tailleAvecUneBordure);

        // #tp3 appel la fonction qui va verifier les positions restantes qui sont libres
        TrouverPosLibres();

        // #tp3 appel la fonction qui va placer les billets et les bonus sur la scene
        PlacerResteObjetsSurScene(_argent, probabiliteArgent, "Argent");
        PlacerResteObjetsSurScene(_pillule, probabiliteBonus, "Bonus");
    }

    /// <summary>
    /// #synthese fonction qui permet la creation du niveau en intentiant les salles et en y placant les objets important.
    /// contient le code qui fesait la creation du niveau dans le awake
    /// </summary>
    /// <param name="tailleAvecUneBordure">taille total que le niveau va avoir</param>
    void CreerSalles(Vector2Int tailleAvecUneBordure)
    {
        // #tp4 si le niveau est different de 1...
        if(_donneesPerso.niveau != 1)
        {
            // #tp4 la taille du niveau en x est egale au niveau + 1
            _taille.x = _donneesPerso.niveau + 1;
        }
        // #tp4 si le niveau est superieur ou egal a 3...
        if(_donneesPerso.niveau >= 3)
        {
            // #tp4 la taille du niveau en y est egale a 3
            _taille.y = 3;
        }

        // #synthese definit le nombre de salle qui devrait y avoir dans le niveau
        _nbSalles = _taille.x * _taille.y;
        // #synthese appelle la fonction dans SODifficulte qui permet de changer les valeurs de certaines variables selon le mode de jeu
        _donneesDifficulte.ChangerDifficulte();

        // #tp3 va chercher diverses valeurs de position pour permettre les objets d'etre instantier au bon endroit
        int posYAlea = UnityEngine.Random.Range(0,_taille.y);
        int posYAleaPorte = UnityEngine.Random.Range(0,_taille.y);
        int posXMax = _taille.x - 1;
        int posXPerso = posXMax - 1;

        // pour chaque salle sur l'axe des x ainsi que chaque salle sur l'axe des y...
        for(int x = 0; x<_taille.x; x++)
        {
            for (int y = 0; y < _taille.y; y++)
            {

                // #synthese definit un bool qui permet de savoir si la salle en cours devrait contenir un ennemie
                bool salleContientEnnemie;
                // #synthese si le nombre d'ennemie est egal au nombre de salles, toutes les salles contient un ennemie (bool reste true)
                if(_nbEnnemis == _nbSalles)
                {
                    salleContientEnnemie = true;
                }
                // #synthese sinon...
                else
                {
                    // #synthese fait un tirage entre 0 et 1
                    int probabiliteApparition = UnityEngine.Random.Range(0,2);
                    // #synthese si le resultat du tirage est 1, definit que la salle va contenir un ennemie
                    if(probabiliteApparition == 1) salleContientEnnemie = true;
                    // #synthese sinon, definit que la salle ne contiendra pas d'ennemie
                    else salleContientEnnemie = false;
                }
                
                // va chercher un numero aleatoire selon le nombre de salle dans le tableau des salles
                int salleAlea = UnityEngine.Random.Range(0,_tSallesModeles.Length);
                // definit la position de chaque salle selon leur taille et leur place dans le niveau
                Vector2 pos = new Vector2(tailleAvecUneBordure.x * x , tailleAvecUneBordure.y * y);
                // instancie la salle aleatoire a la position donnee sur le niveau
                Salle salle = Instantiate(_tSallesModeles[salleAlea], pos, quaternion.identity, transform);

                // #tp4 definit le fond des salles selon leur position sur l'axe des y
                if(y == 0) salle.ChangerFond(_fondBas);
                else salle.ChangerFond(_fondHaut);

                // renomme la salle selon son positionnement dans le niveau
                salle.name = "Salle_"+x+"_"+y;

                // #tp3 va chercher le decalage necessaire pour que les objets apparaissent a la bonne place au niveau de la tilemap
                Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
                // #tp3 si la salle se situe a 0 sur l'axe des x et qu'elle a la valeur aleatoireY comme position sur l'axe des y...
                if(x == 0 && y == posYAlea)
                {

                    PlacerObjetsImportantSurScene(_cle, decalage, salle);

                }
                // #tp3 si la salle se retrouve tout au bout des positions sur l'axe des x et qu'elle a la valeur aleatoireY pour la porte comme position sur l'axe des y...
                if(x == posXMax && y == posYAleaPorte)
                {

                    PlacerObjetsImportantSurScene(_porte, decalage, salle);

                }
                // #tp3 si la salle se retrouve a la position du perso sur l'axe des x et qu'elle a la valeur aleatoireY pour la porte comme position sur l'axe des y...
                if(x == posXPerso && y == posYAleaPorte)
                {

                    PlacerObjetsImportantSurScene(_perso, decalage, salle);

                }
                // #tp3 si la salle ne contient ni porte ou cle , et que l'activateur n'est pas deja present sur scene...
                if(salle.contientClePorte==false && _activateurDejaPresent == false)
                {

                    PlacerObjetsImportantSurScene(_activateur, decalage, salle);

                }

                // #tp3 on appel la fonction qui va instantier un effector dans chacune des salles
                PlacerObjetsImportantSurScene(_effector, decalage, salle);

                // #synthese si la salle doit contenir un ennemie et qu'il reste des ennemies a faire apparaitre...
                if(salleContientEnnemie && _nbEnnemis > 0)
                {
                    // #synthese appelle la fonction dans Salle qui permet de faire apparaitre les ennemies dans les salles
                    salle.FaireApparaitreEnnemies(salleContientEnnemie);
                    // #synthese si le nombre d'ennemies a faire apparaitre n'est pas égal au nombre de salles...
                    if(_nbEnnemis != _nbSalles)
                    {
                        // #synthese on diminue le nombre d'ennemies a faire apparaitre
                        _nbEnnemis--;
                    }
                }
            }
        }
    }

    /// <summary>
    /// #synthese fonction qui permet d'instantier les objets comme l'activateur, la clé, la porte, et le perso dans les salles qui
    /// leur sont assignée. contient le code que chaque salle avait pour instantier les objets en question et modifier pour etre plus generique
    /// </summary>
    /// <param name="objet">GameObject de ce qui doit être instantier</param>
    /// <param name="decalage">decalage necessaire pour que les objets apparaissent a la bonne place au niveau de la tilemap</param>
    /// <param name="salle">salle dans laquelle l'objet doit être instantier</param>
    void PlacerObjetsImportantSurScene(GameObject objet, Vector2Int decalage, Salle salle)
    {
        // #synthese si l'objet a instantier ne porte pas le nom de Perso_Full...
        if(objet.name != "Perso_Full")
        {
            // #tp3 on appel la fonction qui va instantier l'objet fournie sur la scene
            Vector2Int posRep = salle.PlacerSurRepere(objet) - decalage;
            // #tp3 on ajoute la position de l'objet qui a ete retourne a la liste des positions sur reperes
            _lesPosSurRepere.Add(posRep);
            // #synthese si l'objet est la cle ou la porte, definit que cette salle contient deja quelque chose, alors l'activateur n'y ira pas
            if(objet.name == "porte" || objet.name == "cle") salle.contientClePorte = true;
            // #synthese si l'objet est l'activateur on definit que l'activateur est deja present sur la scene
            if(objet.name == "activateur") _activateurDejaPresent = true;
        }
        // #synthese si l'objet a instantier porte le nom de Perso_Full...
        else if(objet.name == "Perso_Full")
        {
            // #tp3 on appel la fonction qui va instantier le perso sur la scene
            Vector2Int posRep = salle.PlacerPerso(objet) - decalage;
            // #tp3 on ajoute la position du perso qui a ete retourne a la liste des positions sur reperes
            _lesPosSurRepere.Add(posRep);
            // #tp3 donne le script Salle pour que le texte retroactif puisse etre afficher aux bonnes places
            sallePerso = salle.GetComponent<Salle>();
        }
        
    }

    /// <summary>
    /// #synthese fonction qui permet de fermer le niveau en placant des tuiles la ou les bordures du niveau ce trouve.
    /// contient le code qui était present dans le awake
    /// </summary>
    /// <param name="tailleAvecUneBordure">taille total que le niveau va avoir</param>
    void FermerNiveau(Vector2Int tailleAvecUneBordure)
    {
        // #tp4 definit le confiner du niveau et va chercher collider de type polygon du niveau
        PolygonCollider2D confiner = GetComponent<PolygonCollider2D>();

        // variables qui définit la taille que le niveau a, ainsi que la valeur minimal et maximal de sa taille
        Vector2Int tailleNiveau = _taille * tailleAvecUneBordure;
        Vector2Int min = Vector2Int.zero - Salle.taille / 2;
        Vector2Int max = min + tailleNiveau;

        // pour chaques cases dans les axes des y et des x...
        for(int y = min.y; y <= max.y; y++)
        {
            for(int x = min.x; x <= max.x; x++)
            {
                // cree une position selon la tuile qui est en train de se faire verifier
                Vector3Int pos = new Vector3Int( x, y);

                // #synthese AVANT "if(x == min.x || x== max.x) tilemap.SetTile(pos, _tuileMurs);
                //                  if(y == min.y || y== max.y) tilemap.SetTile(pos, _tuileMurs);"
                //MAINTENANT "if(x == min.x || x== max.x || y == min.y || y== max.y) tilemap.SetTile(pos, _tuileMurs);"
                // si cette tuile ce situe aux extremités du niveau, elle est remplacer par la tuile de mur
                if(x == min.x || x== max.x || y == min.y || y== max.y) tilemap.SetTile(pos, _tuileMurs);
            }
        }

        // #tp4 met a jour la taille du confiner du niveau selon les bornes de la tilemap
        confiner.points = new Vector2[] {new Vector2(min.x, min.y), new Vector2(min.x, max.y+1), new Vector2(max.x+1, max.y+1), new Vector2(max.x+1, min.y)};
    }

    // #synthese ajout des parametre decalage, pos et tuile au summary de la fonction
    /// <summary>
    /// Fonction qui recoit les tuiles envoye par la classe CarteTuiles et les rajoute au tilemap du niveau
    /// </summary>
    /// <param name="decalage">Decalage a definir pour que les tuiles puissent apparaitrent aux bonnes places dans le niveau</param>
    /// <param name="pos">Position de la tuile a traiter</param>
    /// <param name="tuile">Tuile qui sera placé selon la position et le décalage fournis</param>
    public void CreerTuile(Vector3Int decalage, Vector3Int pos, TileBase tuile)
    {
        tilemap.SetTile(pos + decalage, tuile);
    }

    // #synthese ajout des parametre objet, probabilite et nom au summary de la fonction
    /// <summary>
    /// #tp3 fonction qui va placer l'argent et les bonus dans la scene selon les positions libres qui restent
    /// </summary>
    /// <param name="objet">model de l'objet a etre instancier sur le niveau</param>
    /// <param name="probabilite">nombre aleatoire a fournir qui definit le nombre total d'instance de l'objet il va y avoir</param>
    /// <param name="nom"></param>
    void PlacerResteObjetsSurScene(GameObject objet, int probabilite, string nom)
    {
        // #tp3 crée un gameobject vide ou on va mettre toute les instances de ce qui est en train detre instantier dedans
        Transform conteneur = new GameObject(nom).transform;
        conteneur.parent = transform;
        // #tp3 tant que i n'est pas egal a la probabilité de l'objet...
        for(int i = 0; i < probabilite; i++)
        {
            // on appel la fonction qui va chercher une position libre
            Vector2Int pos = ObtenirUnePosLibre();
            // #tp3 on cree une position selon la position trouver et la position de la tilemap
            Vector3 pos3 = (Vector3)(Vector2)pos + _tilemap.transform.position + _tilemap.tileAnchor;

            // #synthese change la position en z pour que les particules puissent jouer corectement (particules 3D dans jeu 2D)
            pos3.z = -2;

            // #tp3 l'objet se fait instantier a la cette position, dans le gameobject vide 
            Instantiate(objet, pos3, Quaternion.identity, conteneur);

            // #tp4 si l'objet instantier est un bonus, on ajoute 1 au nombre de bonus sur la scene
            if(nom == "Bonus") _nbPillulesSurScene++;
        }
    }

    /// <summary>
    /// #tp3 fonction qui permet de donner une position libre dans la tilemap aleatoire
    /// </summary>
    Vector2Int ObtenirUnePosLibre()
    {
        // #tp3 un va chercher un nombre aleatoire entre 0 et la valeur maximal de la liste des positions libres
        int indexPosLibre = UnityEngine.Random.Range(0, _lesPosLibres.Count);
        // #tp3 on prend la position qui se trouve a cet place aleatoire dans la liste
        Vector2Int pos = _lesPosLibres[indexPosLibre];
        // #tp3 on retire cette position de la liste
        _lesPosLibres.RemoveAt(indexPosLibre);
        // #tp3 on retourne la position libre
        return pos;
    }

    /// <summary>
    /// #tp3 fonction qui permet de trouver les positions libres dans la tilemap selon les positions sur reperes et les tuiles presentes
    /// </summary>
    void TrouverPosLibres()
    {
        // #tp3 on cree une double boucle afin de verifier chaque cases dans la tilemap celon la taille fournie par les bornes de la tilemap
        BoundsInt bornes = _tilemap.cellBounds;
        for (int x = bornes.xMin; x < bornes.xMax; x++)
        {
            for (int y = bornes.yMin; y < bornes.yMax; y++)
            {
                // #tp3 on enregistre la position de la tuile
                Vector2Int posTuile = new Vector2Int (x , y);
                // #tp3 on verifie si sur cette tuile il y a deja une tuile de niveau ou non
                TileBase tuile = _tilemap.GetTile((Vector3Int)posTuile);
                // #tp3 si il ny a pas de tuile a la position verifier...
                if(tuile == null)
                {
                    // #tp3 on rajoute cette position dans la liste des positions libres
                    _lesPosLibres.Add(posTuile);
                }
            }
        }
        // #tp3 pour chaque position dans la liste des positions sur reperes...
        foreach (Vector2Int pos in _lesPosSurRepere)
        {
            // #tp3 on retire cette position de la liste des positions libres
            _lesPosLibres.Remove(pos);
        }
    }

    // #synthese ajout du parametre texte au summary de la fonction
    /// <summary>
    /// #tp3 fonction qui permet d'instancier le champ de texte retroactif quand le joueur interagie avec un objet de la scene
    /// </summary>
    /// <param name="texte">texte a fournir pour qu'il puisse etre afficher dans le champ de texte de la retroaction</param>
    public void AfficherTexteRetroactif(string texte)
    { 
        // #tp3 on va chercher la position actuelle ou le perso ce trouve en ce moment grace a l'instance du perso sauvegarder dans Salle
        Vector2 pos = sallePerso.persoInstance.transform.position;
        // #tp3 on instantie le champ de texte a la position ou le perso etait tout en gardant une reference du champ de texte
        Retroaction retroaction = Instantiate(_modeleRetroaction, pos, Quaternion.identity, transform.parent);
        // #tp3 un utilise la reference afin d'appeller la fonction qui va changer le texte dans le champ de texte
        retroaction.ChangerTexte(texte);
    }

    /// <summary>
    /// #tp4 fonction qui permet de jouer un son selon le nom du clip
    /// </summary>
    /// <param name="nomClip">string du nom du son a jouer</param>
    public void JouerSon(string nomClip)
    {
        // #tp4 selon le nom du clip, on joue le son correspondant
        switch (nomClip)
        {
            // #tp4 si le nom du clip est "atterissage" on joue le son d'atterissage
            case "atterissage":
                _audio.PlayOneShot(_sonAtterissage);
                break;
            // #tp4 si le nom du clip est "saut" on joue le son de saut
            case "saut":
                _audio.PlayOneShot(_sonSaut);
                break;
            // #tp4 si le nom du clip est "porte" on joue le son de porte
            case"porte":
                _audio.PlayOneShot(_sonPorte);
                break;
            // #tp4 si le nom du clip est "cle" on joue le son de cle
            case "cle":
                _audio.PlayOneShot(_sonCle);
                break;
        }
    }

    /// <summary>
    /// #tp4 fonction qui permet de detruire le perso pour pas que le joueur puisse continuer a jouer meme si le niveau est fini
    /// </summary>
    public void DetruirePerso()
    {
        // #tp4 "detruit" le perso
        sallePerso.persoInstance.SetActive(false);
    }

    /// <summary>
    /// #synthese fonction qui permet aux ennemies apres leur mort de faire apparaitre un bonus (ou non) que le joueur peut ramasser
    /// </summary>
    /// <param name="pos">position que l'ennemie fournie après sa mort. il s'agit de sa derniere position connue</param>
    public void DonnerBonusMort(Vector2 pos)
    {
        // #synthese augmente le nombre d'ennemie que le joueur a tué par 1
        _ennemisTue++;
        // #synthese fait un tirage dans chiffre entre 0 et 5
        int probabilite = UnityEngine.Random.Range(0, 6);
        // #synthese si jamais la variable probabilite a comme valeur...
        switch (probabilite)
        {
            // #synthese 1,2 ou 5...
            case 1:
            case 2:
            case 5:
                // #synthese instantie 3 fois un billet d'argent la ou l'ennemmie était
                // et fais en sorte que chanque instance ai +1 a leur x comparer au precedent
                Instantiate(_argent, pos, Quaternion.identity);
                Instantiate(_argent, pos + new Vector2(1,0), Quaternion.identity);
                Instantiate(_argent, pos + new Vector2(2,0), Quaternion.identity);
                // #synthese fin de la "branche"
                break;
            // #synthese 0 ou 4...
            case 0:
            case 4:
                // #synthese instantie une pillule la ou l'ennemie était
                Instantiate(_pillule, pos, Quaternion.identity);
                // #synthese fin de la "branche"
                break;
            // #synthese 3...
            case 3:
                // #synthese fin de la "branche" (le joueur ne gagne rien)
                break;
        }
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        // #tp4 remet aux valeurs initiales les données du perso et desactive ses bonus
        _donneesPerso.Initialiser();
        _donneesPerso.InitialiserBonus();
        // #tp4 remet les valeurs de pointages a zero
        _donneesPointage.Initialiser();

        // #synthese remet le mode de jeu par defaut a normal si le joueur quitte le jeu
        _donneesDifficulte.modeDifficile = false;

    }
}