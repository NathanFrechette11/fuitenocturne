using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #synthese Classe a ajouter sur le prefab des ennemies.
/// Elle permet aux ennemis de se déplacer, sauter et vérifier s'ils sont au sol
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>
public class Ennemie : MonoBehaviour
{
    // definit La puissance à laquelle l'ennemie va sauter
    [SerializeField] float _puissanceSaut = 5f; 
    // definit Le nombre de frames pendant lesquelles l'ennemie peut sauter
    [SerializeField] int _nbFramesMax = 6; 
    // definit Nombre de frames de saut restantes pendant le saut
    int _nbFramesRestants = 0; 

    [Header("Propriétés génériques")]
    // definit Destinations pour le trajet
    [SerializeField] Transform[] _tDest; 
    // definit Permet l'accès public aux destinations
    public Transform[] tDest => _tDest; 
    // definit Délai avant le premier départ
    [SerializeField] float _delaiPremierDepart = 0.5f; 
    // definit Délai entre les départs suivants
    [SerializeField] float _delaiDepartsSuivants = 1f; 

    [Header("Propriétés AddForce")]
    // definit Vitesse maximale de déplacement
    [SerializeField] float _vitesseMax = 5f; 
    // definit Force a appliquée pour le déplacement
    [SerializeField] float _force = 6f; 
    // definit Distance de freinage avant d'atteindre la destination
    [SerializeField][Range(0.1f, 3f)] float _distFreinage = 2f; 
    // definit Tolérance pour considérer que la destination est atteinte
    [SerializeField] float _toleranceDest = 0.1f; 
    // index de la destination a prendre
    int _iDest = 0;
    // definit le spriteRenderer de l'ennemie
    SpriteRenderer _sr;
    // definit le rigibBody de l'ennemie
    Rigidbody2D _rb;
    // definit l'animator de l'ennemie
    Animator _anim;
    // definit le collider de l'ennemie
    Collider2D _coll;

    // variable qui définit l'axe en x de l'ennemie quand il bouge
    float _axeHorizontal;
    // definit les données du mode de jeu (donnée de la difficulté)
    [SerializeField] SODifficulte _donneesDifficulte;
    // definit le nombre de vie que l'ennemie a
    [SerializeField] int _vie = 3;

    void Awake()
    {
        // si le mode de jeu est en mode difficile...
        if(_donneesDifficulte.modeDifficile)
        {
            // la force de saut des ennemies est multiplié par 10
            _force = _force*10f;
            // leur vie est doublée
            _vie = 6;
        }
        // sinon...
        else
        {
            // leur force de saut reste pareille
            _force = 6f;
            // leur nombre de vie reste pareille
            _vie = 3;
        }

        // va chercher la composante RigidBody2D
        _rb = GetComponent<Rigidbody2D>();
        // va chercher la composante Animator
        _anim = GetComponent<Animator>();
        // va chercher la composante Collider2D
        _coll = GetComponent<Collider2D>();
        // va chercher la composante SpriteRenderer
        _sr = GetComponent<SpriteRenderer>();

        // si la taille du tableau des destinations est plus grande que 0...
        if (_tDest.Length > 0)
        {
            // definit quelle est la position que le rigidbody doit atteindre
            _rb.MovePosition(new Vector2(_tDest[_iDest].position.x, transform.position.y));
            // commence la coroutine qui permet de gerer les trajets des ennemies
            StartCoroutine(CoroutineGererTrajet());
        }
        // sinon...
        else
        {
            // affiche dans la console qu'il n'y a aucune destination
            Debug.LogError("Aucune destination assignée à l'ennemi.");
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        // definit que la valeur de axeHorizontal est la valeur du x de la velocite de l'ennemie
        _axeHorizontal = _rb.velocity.x;
        // si l'axe horizontal est plus petit que 0, le sprite de l'ennemie est tourné vers la gauche
        if(_axeHorizontal<0){
            _sr.flipX = true;
        }
        // par contre s'il est plus grand que 0, on laisse le sprite a son état normal puisqu'il regarde déja vers la droite
        else if(_axeHorizontal>0){
            _sr.flipX = false;
        }
        // si la vie de l'ennemie atteint 0...
        if(_vie == 0)
        {
            // enregistre la derniere position que l'ennemie avait avant de mourir
            Vector2 pos = transform.position;
            // appelle la fonction dans la classe niveau qui permet de faire apparraitre (ou non) un bonus a la place de l'ennemie
            Niveau.instance.DonnerBonusMort(pos);
            // detruit l'ennemie
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// coroutine qui permet aux ennenies de ce déplacer d'un point a l'autre, attends .5 seconde avant de commencer, et attends 1 seconde
    /// entre chaque trajet avant de commencer a se deplacer vers un autre point
    /// </summary>
    /// <returns>attends pendant soit 0.5 seconde, soit 1 seconde, ou attend chaque fixedupdate</returns>
    IEnumerator CoroutineGererTrajet()
    {
        // attend avant de commencer selon la duree definit pas delaiPremierDepart
        yield return new WaitForSeconds(_delaiPremierDepart);
        // tant que la coroutine(?) joue...
        while (true)
        {
            // appelle la fonction qui permet d'obtenir la prochaine destination et donne la valeur de la position a la variable posDest
            Vector2 posDest = ObtenirPosProchaineDestination();
            // tant que l'ennemie a toujours pas atteint sa destination...
            while (Mathf.Abs(transform.position.x - posDest.x) > _toleranceDest)
            {
                // verifie qu'on est dans un fixedupdate
                yield return new WaitForFixedUpdate();
                // appelle la fonction qui permet de déplacer l'ennemie vers la destination fournie
                AjouterForceVersDestination(posDest);
            }
            // attend avant de deplacer l'ennemie vers la prochaine destination selon la duree definit pas delaiDepartsSuivants
            yield return new WaitForSeconds(_delaiDepartsSuivants);
        }
    }

    /// <summary>
    /// fonction qui permet d'appliquer une force qui permettra a l'ennemie de ce
    /// deplacer vers la position de la destination qu'il doit atteindre
    /// </summary>
    /// <param name="posDest">position de la destination que l'ennemie doit atteindre</param>
    private void AjouterForceVersDestination(Vector2 posDest)
    {
        // definit la position actuelle de l'ennemie
        Vector2 positionActuelle = transform.position;
        // definit la direction que l'ennemie doit aller selon la position en x de la destination et la position de l'ennemie en x
        Vector2 direction = new Vector2(posDest.x - positionActuelle.x, 0).normalized;
        
        // Calculer la distance en ne prenant en compte que la différence en X
        float distance = Mathf.Abs(posDest.x - positionActuelle.x);

        // Calculer le ratio de freinage
        float ratioFreinage = (distance > _distFreinage) ? 1 : distance / _distFreinage;

        // Ajouter la force seulement dans la direction horizontale (X)
        _rb.AddForce(direction * _force);
        _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -_vitesseMax * ratioFreinage, _vitesseMax * ratioFreinage), _rb.velocity.y);
    }

    /// <summary>
    /// fonction qui permet d'obtenir la destination a laquelle l'ennemie doit ce rendre
    /// </summary>
    /// <returns>retourne un vector2 de la position de la destination qui ce trouve dans le tableau</returns>
    Vector2 ObtenirPosProchaineDestination()
    {
        // augmente la valeur de l'index selon la longueur du tableau des destinations, et le remet a 0 si on atteint la fin du tableau
        _iDest = (_iDest + 1) % _tDest.Length;
        // retourne la position de la destination qui ce trouve dans le tableau des destinations et qui est a la meme position que l'index
        return new Vector2(_tDest[_iDest].position.x, transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // si l'ennemie entre en contact avec l'objet qui lui permet de sauter
        if (other.CompareTag("SautEnnemie"))
        {
            // definit que le nombre de frames de saut restant est egal au nombre de frames de saut maximal
            _nbFramesRestants = _nbFramesMax;
            // definit que la fraction de force de saut est egal aux frames restants divisé par les frames maximal
            float fractionForce = (float)_nbFramesRestants / _nbFramesMax;

            // definit un vecteur qui pointe vers le haut multiplier par la puissance de saut de l'ennemie et la fraction de force
            Vector2 vecteurForce = Vector2.up * _puissanceSaut * fractionForce;
            // ajoute la force du vecteur de force au rigibbody de l'ennemie
            _rb.AddForce(vecteurForce, ForceMode2D.Impulse);

            // si le nombre de frame restant est plus grand que 0, le nombre de frames restant diminue
            if (_nbFramesRestants > 0) _nbFramesRestants--;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // definit que le layer a verifier est celui du couteau
        int couteauLayerIndex = LayerMask.NameToLayer("couteau");
        // si l'ennemie entre en collision avec le couteau...
        if (other.gameObject.layer == couteauLayerIndex)
        {
            // l'ennemie perd de la vie
            _vie--;
        }
    }
}
