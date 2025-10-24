using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    // dont destroy on load
    #region Singleton
    public static CanvasScript instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CanvasScript found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
