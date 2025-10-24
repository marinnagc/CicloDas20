using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PushButtonElevator : MonoBehaviour
{
    public static event Action<string> OnButtonPressed = delegate { };
    public AudioSource audioSource;

    private int deviderPosition;
    private string buttonName, buttonValue;

    private void Start()
    {
        buttonName = gameObject.name;
        deviderPosition = buttonName.IndexOf("_");
        buttonValue = buttonName.Substring(0, deviderPosition);

        gameObject.GetComponent<Button>().onClick.AddListener(PlayButton);
    }

    private void PlayButton()
    {
        Debug.Log("Button pressed: " + buttonValue);
        audioSource.Play();
        OnButtonPressed(buttonValue);
    }
}
