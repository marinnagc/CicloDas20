using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PasswordPadDisplay : MonoBehaviour
{
    public GameObject passwordInteraface;

    public TMP_InputField passwordInputField;

    public TextMeshProUGUI incorrectMessage;

    public Button submitButton;

    public string correctPassword = "12345";


    public void Start()
    {
        // passwordInteraface.SetActive(false);
        incorrectMessage.text = "";
    }

    public void OpenPasswordInterface()
    {
        passwordInteraface.SetActive(true);
    }

    public void ClosePasswordInterface()
    {
        passwordInteraface.SetActive(false);
    }

    public void SubmitPassword()
    {
        string inputPassword = passwordInputField.text;

        if (inputPassword == correctPassword)
        {
            CloseInterface();

            SceneManager.LoadScene("WinFinalScreen");
        }
        else
        {
            incorrectMessage.text = "Incorrect Password";
        }
    }

    public void Update()
    {
        if (passwordInputField.text.Length > 0)
        {
            submitButton.interactable = true;
        }
        else
        {
            submitButton.interactable = false;
        }
    }

    public void CloseInterface()
    {
        incorrectMessage.text = "";
        passwordInputField.text = "";
        passwordInteraface.SetActive(false);

        PlayerMovement.SetCurrentInteractable(null);
        PlayerMovement.UnfreezePlayer();
    }
}
