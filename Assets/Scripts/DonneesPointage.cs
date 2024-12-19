[System.Serializable]

/// <summary>
/// #tp4 classe qui permet de creer mon propre systeme de donnees a enregistrer pour la sauvegarde des donnees.
/// a comme attribut le nom du joueur, son score, et permet de savoir s'il s'agit du score du joueur actuel
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class DonneesPointage
{
    public string nom;
    public int pointage;
    public bool joueurActuel;
}