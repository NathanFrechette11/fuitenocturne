using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinitialisationDifficulte : MonoBehaviour
{
    [SerializeField] SODifficulte _donneesDifficulte;

        /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        _donneesDifficulte.modeDifficile = false;
    }
}
