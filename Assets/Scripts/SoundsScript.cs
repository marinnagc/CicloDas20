using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsScript : MonoBehaviour
{
    // dont destroy on load
    #region Singleton
    public static SoundsScript instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of SoundsScript found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
