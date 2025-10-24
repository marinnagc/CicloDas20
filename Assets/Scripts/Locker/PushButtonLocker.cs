using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PushButtonLocker : MonoBehaviour
{
    public Button increaseButton;
    public Button decreaseButton;
    public TextMeshProUGUI displayText;
    public AudioSource audioSource;

    public string charactersToUse;

    private int currentCharacterIndex;

    private string currentCharacter;

    public static event Action<string> OnButtonPressed;

    public string GetCurrentCharacter()
    {
        return currentCharacter;
    }

    void Start()
    {
        increaseButton.onClick.AddListener(IncreaseCharacter);
        decreaseButton.onClick.AddListener(DecreaseCharacter);
        currentCharacterIndex = 0;

        currentCharacter = charactersToUse[currentCharacterIndex].ToString();

        displayText.text = currentCharacter;
    }

    private void IncreaseCharacter()
    {
        audioSource.Play();

        currentCharacterIndex++;
        if (currentCharacterIndex >= charactersToUse.Length)
        {
            currentCharacterIndex = 0;
        }
        currentCharacter = charactersToUse[currentCharacterIndex].ToString();

        displayText.text = currentCharacter;
        OnButtonPressed?.Invoke(currentCharacter);
    }

    private void DecreaseCharacter()
    {
        audioSource.Play();
        
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex = charactersToUse.Length - 1;
        }

        currentCharacter = charactersToUse[currentCharacterIndex].ToString();

        displayText.text = currentCharacter;
        OnButtonPressed?.Invoke(currentCharacter);
    }

    void OnDestroy()
    {
        increaseButton.onClick.RemoveListener(IncreaseCharacter);
        decreaseButton.onClick.RemoveListener(DecreaseCharacter);
    }
}
