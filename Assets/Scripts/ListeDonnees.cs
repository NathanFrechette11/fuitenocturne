using System.Collections.Generic;
[System.Serializable]

/// <summary>
/// #tp4 Classe qui est donner quand on veut enregistrer ou lire les valeurs en JSON afin d'avoir acces au données de la sauvegarde
/// Auteur du code: Nathan Fréchette
/// Auteur des commentaires : Nathan Fréchette
/// </summary>

public class ListeDonnees
{
    public List<DonneesPointage> donnees;

    public ListeDonnees()
    {
        donnees = new List<DonneesPointage>();
    }
}