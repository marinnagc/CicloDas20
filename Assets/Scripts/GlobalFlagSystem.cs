using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlagSystem : MonoBehaviour
{
    #region Singleton
    public static GlobalFlagSystem instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GlobalFlagSystem found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public bool gameStarted = false;
    public bool elevatorUnlocked = false;
}
