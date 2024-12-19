using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #tp4 Classe a ajouter sur les differentes pistes audio qui sont dans les enfants de gestionnaire audio.
/// Elle permet de faire partir toutes les pistes en meme temps, de les mettres en sourdine ou non quand il le faut,
/// et de faire les fondus sonores quand les pistes "commencent" et "terminent"
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class PisteMusicale : MonoBehaviour
{
    // permet de definir le type de la piste a partir de l'enum TypePiste dans l'inspecteur et cree un getteur pour que d'autre scripts y ait acces
    [SerializeField] TypePiste _type; 
    public TypePiste type => _type;
    // permet de definir si une piste est active des le debut ou si elle est active plus tard
    [SerializeField] bool _estActifParDefaut; 
    [SerializeField] bool _estActif; 
    // cree un getter setter pour definir a partir de dautre scripts si la piste est active ou non
    public bool estActif
    {
        get => _estActif;
        set
        {
            _estActif = value;
            // appelle la fonction qui ajuste le volume des pistes
            AjusterVolume();
        }
    }

    // definit l'audioSource tout en creant un accesseur afin de l'utiliser dans d'autre script
    AudioSource _source;
    public AudioSource source => _source;

    void Awake() 
    {
        // va chercher l'audio source qui ce situe dans les pistes audio
        _source = GetComponent<AudioSource>();
        // definit la valeur de _estActif avec si elle doit etre active au depart
        _estActif = _estActifParDefaut;
        // fait partir la musique des que la classe se lance et la fait jouer en boucle
        _source.loop = true;
        _source.playOnAwake = true;
    }

    
    void Start()
    {
        // appelle la fonction qui ajuste le volume des pistes
        AjusterVolume();
    }

    /// <summary>
    /// fonction qui permet d'ajuster le volume des pistes (les rendres audibles si elles sont actives, ou les 
    /// rendre muettent si elle ne sont pas active)
    /// </summary>
    public void AjusterVolume() 
    {
        // si la piste est active, fait partir la coroutine qui fait les fondus audio en lui envoyant un volume positif et une duree de 1 seconde
        if (_estActif)
        {
            StartCoroutine(CoroutineFaireFondu(1f, GestionnaireAudio.instance.volumeMusicalRef));
        }
        // si la piste est inactive, fait partir la coroutine qui fait les fondus audio en lui envoyant un volume de 0 et une duree de 1 seconde
        else
        {
            StartCoroutine(CoroutineFaireFondu(1f, 0f));
        }
    }

    /// <summary>
    /// coroutine que permet au piste audio de commencer doucement et de finir doucement, fait un fade-in fade-out
    /// des pistes audio selon le volume et la duree envoyer
    /// </summary>
    /// <param name="duree">definit la duree sur laquelle la fade-in/fade-out doit ce faire</param>
    /// <param name="volCible">definit le volume que l'on veut atteindre</param>
    /// <returns>rien</returns>
    IEnumerator CoroutineFaireFondu(float duree, float volCible)
    {
        // garde en memoire le volume que la piste a avant le fondu
        float volumeIni = _source.volume;
        // mets le temps ecoule a 0
        float tempsEcoule = 0f;
        // tant que le temps ecoule est plus petit que la duree sur laquelle faire le fondu...
        while (tempsEcoule < duree)
        {
            // rajoute du temps au tempsecoule selon le temps reel
            tempsEcoule += Time.deltaTime;
            // divise le temps ecoule par la duree a atteindre
            float fraction = tempsEcoule / duree;
            // permet de faire le fondu selon la valeur initial du volume, la valeur du volume qu'on veut, et la fraction de temps qui reste
            _source.volume = Mathf.Lerp(volumeIni, volCible, fraction);
            // ne retourne rien
            yield return null;
        }
        // a la fin, le volume de la piste est egal au volume voulu
        _source.volume = volCible;
    }
}