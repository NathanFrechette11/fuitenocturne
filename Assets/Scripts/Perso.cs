using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

/// <summary>
/// Classe a ajouter sur le personnage pour le contrôler.
/// Elle permet au personnage de se déplacer, sauter et faire un double saut après avoir sauté.
/// Auteurs du code: Nathan Fréchette et Cynthia Bélanger
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class Perso : DetecteurSol
{
    // #synthese variable qui permet de definir le son de degats
    [SerializeField] AudioClip _sonDegats;
    // #synthese variable qui permet de definir le son d'attaque
    [SerializeField] AudioClip _sonAttaque;
    // #synthese variable qui permet de definir l'audio source du perso
    AudioSource _audio;

    // #tp4 definit une variable pour l'animator du perso
    Animator _anim;
    
    // #tp3 donne acces au donnees du perso afin de lui donner les bonus acheter
    [SerializeField] SOPerso _donneesPerso;

    // #synthese definie un dégradé pour quand le bonus est active pour le systeme de particules
    [SerializeField] Gradient _gradientBonus;
    // #synthese definie un dégradé normalpour le systeme de particules
    [SerializeField] Gradient _gradientNormal;

    // #synthese definie le prefab du couteau qui doit aller vers la droite
    [SerializeField] Couteau _couteauDroite;
    // #synthese definie le prefab du couteau qui doit aller vers la gauche
    [SerializeField] Couteau _couteauGauche;
    // #synthese definie le decalage par rapport au personnage que le couteau droit doit avoir
    [SerializeField] Transform _decalageDroite;
    // #synthese definie le decalage par rapport au personnage que le couteau gauche doit avoir
    [SerializeField] Transform _decalageGauche;
    // #synthese definie un bool qui permet de savoir si le perso doit tirer vers la droite
    bool _tireDroit;
    // #synthese definie un bool qui permet de savoir si le perso doit tirer vers la gauche
    bool _tireGauche;

    // #tp3 donne acces a divers modules du systeme de particules
    [SerializeField] ParticleSystem[] _part;
    ParticleSystem.MainModule _main;
    ParticleSystem.VelocityOverLifetimeModule _velocity;
    ParticleSystem.ShapeModule _shape;

    // #synthese donne acces au module de couleur selon la duree de vie du systeme de particule
    ParticleSystem.ColorOverLifetimeModule _colorOverLifetime;

    // #tp3 definie des valeurs pour les bonus, ainsi que les valeurs initials
    [SerializeField] float _puissanceSuperSaut = 3f;
    [SerializeField] float _puissanceSautNormal = 2.5f;
    Vector2 _velociteBonus = new Vector2(1f,3f);
    Vector2 _velociteNormal = new Vector2(-0.05f,0f);

    [SerializeField] float _vitesse = 5f; // vitesse a laquelle le personnage va se déplacer
    [SerializeField] float _puissanceSaut = 2.5f; // la puissance a laquelle le perso va sauter
    [SerializeField] int _nbFramesMax = 6; // le nombre de frame ou le perso va pouvoir faire son saut
    float _axeHorizontal; // variable définit quand le personnage bouge
    Rigidbody2D _rb; // nous permet d'avoir accès au RigidBody du perso une fois définie dans le Start
    SpriteRenderer _sr; // nous permet d'avoir accès au SpriteRenderer du perso une fois définie dans le Start

    int _nbFramesRestants = 0; // nombre de frame de saut qui reste au perso pendant son saut

    bool _peutSauter = false; // si le personnage est au sol, devient true pour qu'il puisse sauter

    // #synthese modification du nom pour qu'il respecte les rêgles
    bool _possedeDoubleSaut = false; // option qui devient True une fois acheté dans la boutique pour permettre le double saut

    // si _doubleSaut est True et que le perso a touché au sol, devient True pour qu'il fasse un 2e saut dans les airs
    bool _peutDoubleSauter = false; 

    // #synthese definit un bool qui permet de savoir si le joueur a deja sauter
    bool _aDejaSaute = false;
    // #synthese definit un bool qui permet de savoir si le premier saut du joueur est termine
    bool _finPremierSaut = false;
    // #synthese definit un temps de recharge avant que le joueur peut tirer a nouveau
    float _tempsRecharge = 2.5f;


    void Start()
    {
        // #synthese va chercher le composant audio source du perso
        _audio = GetComponent<AudioSource>();

        // #tp4 va chercher la composante Animator qui est dans le perso
        _anim = GetComponent<Animator>();
        // #tp4 definie que le son de saut n'a pas deja joué
        Niveau.instance.sonSautDejaJouer = false;

        // #synthese avant il ny avait que _part simple, maintenant on appelle les modules par sa position dans le tableau des particlesystem
        // #tp3 va chercher les composantes de chaque module du systeme de particules
        _part = GetComponentsInChildren <ParticleSystem>();
        _main = _part[0].main;
        _velocity = _part[0].velocityOverLifetime;
        _shape = _part[0].shape;

        // #synthese va chercher le module de couleur selon le temps de vie du particlesystem a la position 0
        _colorOverLifetime = _part[0].colorOverLifetime;
        // #synthese lui donne la couleur du degradé normal
        _colorOverLifetime.color = _gradientNormal;

        // #tp3 va chercher les composantes du rigidbody ainsi que du sprite renderer du perso
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _puissanceSaut = _puissanceSautNormal;
    }

    /// <summary>
    /// Le FixedUpdate "écrase" celui de son parent, donc selui de la classe DetecteurSol pour pouvoir utiliser les 2 FixedUpdate.
    /// Permet au personnage de sauter et de se déplacer quand les touches ont été appuyé.
    /// </summary>
    override protected void FixedUpdate()
    {
        // #tp4 definie les différents parametres de l'animator selon les variables que le perso utilise
        _anim.SetFloat("vitesseX", _axeHorizontal);
        _anim.SetFloat("vitesseY", _rb.velocity.y);
        _anim.SetBool("estAuSol", _estAuSol);
        // #tp4 definit que la valeur de sonSolDejaJouer est egal a la valeur de _estAuSol
        Niveau.instance.sonSolDejaJouer = _estAuSol;

        base.FixedUpdate(); // appelle le FixedUpdate de la classe DetecteurSol pour que le personnage puisse détecter le sol

        // #synthese definit que le temps de recharge diminue selon le temps reel et s'il atteint 0 reste a 0
        _tempsRecharge = _tempsRecharge - Time.fixedDeltaTime;
        if( _tempsRecharge <= 0 ) _tempsRecharge = 0;
        // #synthese si le joueur appuie sur le bouton gauche de la souris et que le temps de recharge est a 0...
        if (Input.GetButton("Fire1") && _tempsRecharge <= 0)
        {
            // #synthese lance l'animation d'attaque du personnage qui lui va appeler la fonction pour lancer les couteaux
            _anim.SetTrigger("attaque");
        }

        // #synthese si le joueur nessait pas de sauter et que le premier saut a deja ete fait, on definit que le perso a deja sauter
        if(_peutSauter == false && _finPremierSaut == true) _aDejaSaute = true;

        // #synthese déplacement des lignes de code dans une autre fonction a part
        if(_peutSauter){ // si le perso veut sauter...
            // #synthese appelle la fonction qui permet au personnage de sauter
            Sauter();
        }

        // sinon s'il est au sol, met son nombre de frame de saut au maximum, et si la propriété de double saut a été débloqué, met
        // _peutDoubleSauter a True
        else if(_estAuSol){

            // #tp4 si le son d'atterissage n'a pas deja jouer, envoie le nom du son a jouer a la classe niveau
            if(Niveau.instance.sonSolDejaJouer == false) Niveau.instance.JouerSon("atterissage");
            // #tp4 definit que le son d'atterissage a deja jouer
            Niveau.instance.sonSautDejaJouer = true;
            
            _nbFramesRestants = _nbFramesMax;

            // #synthese modification du nom, avant "_doubleSaut"  MAINTENANT "_possedeDoubleSaut"
            if(_possedeDoubleSaut) _peutDoubleSauter = true;
            // #synthse definit que le perso n'a pas deja sauter
            _aDejaSaute = false;
        }
        else { // sinon, le nombre de frame de saut devient 0
            _nbFramesRestants = 0;
            }
        // ajoute de la vélocité au personnage selon sa vitesse et l'axe horizontal
        _rb.velocity = new Vector2(_axeHorizontal * _vitesse , _rb.velocity.y);

        // #synthese modification du nom, avant "_doubleSaut"  MAINTENANT "_possedeDoubleSaut"
        // #tp3 la valeur de _possedeDoubleSaut est la meme que celle qui est dans le SOPerso
        _possedeDoubleSaut = _donneesPerso.doubleSaut;

        // #tp3 si la valeur de vitesseEleve est true, la vitesse du perso est plus grande, sinon elle reste la meme
        if(_donneesPerso.vitesseEleve) _vitesse = 8f;
        else _vitesse = 5f;
        // #tp3 si la valeur de superSaut est true, la puissance de saut du perso sera plus grande, sinon elle restera la meme
        if (_donneesPerso.superSaut) {
            _puissanceSaut = _puissanceSuperSaut;
        } else {
            _puissanceSaut = _puissanceSautNormal;
        }
    }

    /// <summary>
    /// #tp3 fonction qui permet de donner au personnage un bonus de saut quand il collecte des bonus sur la scene
    /// </summary>
    public void DonnerBonus()
    {
        // #synthese met la valeur en y de la velocite du systeme de particule egal a la valeur en y de la velocite bonus
        _velocity.y = _velociteBonus.y;
        // #synthese met la valeur en x de la velocite du systeme de particule egal a la valeur en x de la velocite bonus
        _velocity.x = _velociteBonus.x;
        // #synthese met la couleur du systeme de particules egal a celle du degrade bonus
        _colorOverLifetime.color = _gradientBonus;

        // #tp3 definit que le personnage a son bonus de super saut activé
        _donneesPerso.superSaut = true;
    }

    /// <summary>
    /// #tp3 fonction qui permet de retirer au personnage le bonus de saut apres que le temps de 5 secondes soit ecoulé
    /// </summary>
    public void RetirerBonus()
    {
        // #synthese met la valeur en y de la velocite du systeme de particule egal a la valeur en y de la velocite normal
        _velocity.y = _velociteNormal.y;
        // #synthese met la valeur en x de la velocite du systeme de particule egal a la valeur en x de la velocite normal
        _velocity.x = _velociteNormal.x;
        // #synthese met la couleur du systeme de particules egal a celle du degrade normal
        _colorOverLifetime.color = _gradientNormal;

        // #tp3 definit que le personnage a son bonus de super saut désactivé
        _donneesPerso.superSaut = false;
    }

    /// <summary>
    /// Fonction qui est appelé quand le joueur appuie sur les boutons pour déplacer le personnage.
    /// </summary>
    void OnMove(InputValue value)
    {
        // #tp3 fait partir le systeme de particule quand le personnage commence a bouger
        _part[0].Play();

        // #synthese empeche le particlesysteme a la position 1 du tableau des systeme de particules de se déclencher
        _part[1].Stop();

        // va chercher le vecteur horizontal du personnage pour vérifier s'il se déplace vers la droite ou la gauche
        _axeHorizontal = value.Get<Vector2>().x;
        // si l'axe horizontal est plus petit que 0, on "tourne" le sprite du perso pour qu'il regarde vers la gauche
        if(_axeHorizontal<0){

            // #synthese definit que le personnage doit tiree a sa gauche et non a sa droite
            _tireDroit = false;
            _tireGauche = true;

            _sr.flipX = true;
            // #tp3 "tourne" l'orientation vers laquelle les particules sont envoyer pour reste vers le derriere du perso quand il tourne vers la gauche
            _velocity.x = 1.75f;
            // #tp3 change la position de ou les particules vont apparaitre pour qu'ils restent aux pieds du perso quand il est vers la gauche
            _shape.position = new Vector3(1.25f, _shape.position.y, _shape.position.z);
        }
        // par contre s'il est plus grand que 0, on laisse le sprite a son état normal puisqu'il regarde déja vers la droite
        else if(_axeHorizontal>0){
            
            // #synthese definit que le personnage doit tiree a sa droite et non a sa gauche
            _tireDroit = true;
            _tireGauche = false;

            _sr.flipX = false;
            // // #tp3 "tourne" l'orientation vers laquelle les particules sont envoyer pour reste vers le derriere du perso quand il tourne vers la droite
            _velocity.x = -1.75f;
            // // #tp3 change la position de ou les particules vont apparaitre pour qu'ils restent aux pieds du perso quand il est vers la droite
            _shape.position = new Vector3(-1.25f, _shape.position.y, _shape.position.z);
        }
        // sinon...
        else
        {
            // #tp3 on arrete le systeme de particule car le personnage a arrete de bouger
            _part[0].Stop();
        }
        // sinon, quand il bouge plus il garde la dernière orientation qu'il avait
    }


    /// <summary>
    /// #synthese fonction qui est appellé quand l'animation de lancer de couteau atteint un certain point.
    /// permet au joueur de lancer un couteau dans la direction qu'il regarde
    /// </summary>
    public void LancerCouteau()
    {
        // #synthese si le joueur doit tirer vers la droite, instancie le couteau a la droite du perso, a la position du decalage droite
        if(_tireDroit) Instantiate(_couteauDroite, _decalageDroite.position, transform.rotation);
        // #synthese sinon si le joueur doit tirer vers la gauche, instancie le couteau a la gauche du perso, a la position du decalage gauche
        else if (_tireGauche) Instantiate(_couteauGauche, _decalageGauche.position, transform.rotation);
        // #synthese si le mode de jeu est a difficile, le temps de recharge du perso est de 5 secondes
        if(Niveau.instance.donneesDifficulte.modeDifficile) _tempsRecharge = 5f;
        // #synthese sinon, il est de 2 secondes et demi
        else _tempsRecharge = 2.5f;
        // #synthese on joue le son d'attaque
        _audio.PlayOneShot(_sonAttaque);
    }

    /// <summary>
    /// #synthese fonction qui est appeller quand l'animation d'attaque du perso est terminé.
    /// reinitialise le trigger "attaque" de l'animator du perso pour pas qu'il déclenche une 2e fois son animation
    /// </summary>
    public void FinirLancerCouteau()
    {
        _anim.ResetTrigger("attaque");
    }


    /// <summary>
    /// Fonction qui est appelé quand le joueur appuie sur le bouton pour Sauter.
    /// Permet de définir si le booléen _peutSauter True ou non.
    /// Il devient True quand la barre d'espace a été enfoncé.
    /// </summary>
    // #synthese changement nom pour value: avant était "Value", maintenant est "value"
    void OnJump(InputValue value) => _peutSauter = value.isPressed;

    /// <summary>
    /// #synthese fonction qui permet au personnage de faire son saut s'il est au sol et de faire son double saut s'il a deja fait son saut
    /// </summary>
    void Sauter()
    {
        // #tp4 une fois que la valeur de sonSautDejaJouer n'est plus pareille a _estAuSol...
        if(Niveau.instance.sonSautDejaJouer != _estAuSol)
        {
            // #tp4 envoie le nom du son a jouer a la classe niveau
            Niveau.instance.JouerSon("saut");
            // #tp4 change la valeur de sonSautDejaJouer pour sa valeur opposé
            Niveau.instance.sonSautDejaJouer = !Niveau.instance.sonSautDejaJouer;
        }

        float fractionForce = (float)_nbFramesRestants / _nbFramesMax; // définit la fraction de force qu'il a le droit d'utiliser...

        // définit le vecteur de la force de saut selon la force de saut du perso et de la fraction de force qu'il a...
        Vector2 vecteurForce = Vector2.up * _puissanceSaut * fractionForce;
        _rb.AddForce(vecteurForce,ForceMode2D.Impulse); // et applique toute la force de saut sur le personnage en un seul coup

        // tant qu'il reste des frames de saut au personnage, ceux si vont diminuer pour que la fraction de force aussi diminue
        if(_nbFramesRestants > 0) _nbFramesRestants--;

        // #synthese si le nombre de frames restants est egal a 0, on definit que le perso a terminé son premier saut
        if(_nbFramesRestants == 0) _finPremierSaut = true;

        // si le perso peut faire un double saut et une fois qu'il atteint le maximum de son saut, redefinit les frames de sauts
        // restants pour pouvoir refaire un saut, il ne pourra plus refaire de saut avant d'avoir retouché au sol
        // #synthese avant était definit comme "... && _rb.velocity.y <= 0" , maintenant est "... && _aDejaSaute"
        if(_peutDoubleSauter && _aDejaSaute){

            // #synthese definit une variable pour la velocite actuelle du perso et lui fournit la velocite du joueur
            Vector2 velociteActuel = _rb.velocity;
            // #synthese met la valeur en y de cette velocite a 0
            velociteActuel.y = 0;
            // #synthese donne cette velocite a la velocite du joueur pour qu'il reste a la meme place dans les airs lors de son double saut
            _rb.velocity = velociteActuel;
            fractionForce = (float)_nbFramesRestants / _nbFramesMax; // définit la fraction de force qu'il a le droit d'utiliser...

            // définit le vecteur de la force de saut selon la force de saut du perso et de la fraction de force qu'il a...
            vecteurForce = Vector2.up * _puissanceSaut * fractionForce;
            _rb.AddForce(vecteurForce,ForceMode2D.Impulse); // et applique toute la force de saut sur le personnage en un seul coup

            // tant qu'il reste des frames de saut au personnage, ceux si vont diminuer pour que la fraction de force aussi diminue
            if(_nbFramesRestants > 0) _nbFramesRestants--;

            // #tp4 définit que le son de saut n'a pas déja jouer
            Niveau.instance.sonSautDejaJouer = false;

            // #tp4 envoie le nom du son a jouer a la classe niveau
            Niveau.instance.JouerSon("saut");

            // #synthese si le jeu est en mode difficile, le nombre de frames restants pour le double saut diminue de 1
            if(Niveau.instance.donneesDifficulte.modeDifficile) _nbFramesRestants = 5;

            _peutDoubleSauter = false;
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // #synthese si le joueur entre en contact avec la pillule...
        if(other.tag=="Pillule")
        {
            // #synthese appelle la coroutine qui permet de donner et retirer le bonus au joueur
            StartCoroutine(CoroutineBonus());
        }
        // #synthese si le joueur entre en contact avec l'ennemie...
        if(other.tag=="Ennemie")
        {
            // #synthese fait partir le particlesysteme qui est a la position 1 du tableau des systemes de particules
            _part[1].Play();
            // #synthese enleve de la vie au joueur
            _donneesPerso.vie--;

            // #synthese on joue le son de degats
            _audio.PlayOneShot(_sonDegats);
        }
    }

    /// <summary>
    /// #synthese coroutine qui donne le bonus au joueur quand elle est appeler, et apres 5 secondes, retire le bonus
    /// </summary>
    IEnumerator CoroutineBonus()
    {
        // #synthese appelle la fonction qui permet de donner le bonus au joueur
        DonnerBonus(); 
        // #synthese attend pendant 5 secondes
        yield return new WaitForSeconds(5f);
        // #synthese appelle la fonction qui permet de retirer le bonus au joueur
        RetirerBonus();
    }

    /// <summary>
    /// #tp3 quand le joueur quitte le jeu au milieu de la partie, toutes les donnees du personnage, des bonus actifs et du pointage sont reinitialiser
    /// </summary>
    void OnApplicationQuit()
    {
        // #tp3 remet aux valeurs initiales les données du perso
        _donneesPerso.Initialiser();

        // #tp4 permet de reinitialiser les valeurs des bonus accorder au perso
        _donneesPerso.InitialiserBonus();

        // #tp3 remet les valeurs de pointages a zero
        Niveau.instance.donneesPointage.Initialiser();
    }
}