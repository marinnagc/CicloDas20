using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] digits;

    [SerializeField] private Image[] characters;

    private string codeSequence;

    public string codeToUnlock;

    public GameObject uiInterface;

    public AudioSource acceptSound;
    public AudioSource denySound;

    public GameObject sceneManager;

    void Start()
    {
        codeSequence = "";

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].sprite = digits[10];
        }

        PushButtonKeyPad.OnButtonPressed += AddDigitToCodeSequence;
    }

    private void AddDigitToCodeSequence(string digit)
    {
        if (codeSequence.Length < 4)
        {
            switch (digit)
            {
                case "Zero":
                    codeSequence += "0";
                    DisplayCodeSequence(0);
                    break;
                case "One":
                    codeSequence += "1";
                    DisplayCodeSequence(1);
                    break;
                case "Two":
                    codeSequence += "2";
                    DisplayCodeSequence(2);
                    break;
                case "Three":
                    codeSequence += "3";
                    DisplayCodeSequence(3);
                    break;
                case "Four":
                    codeSequence += "4";
                    DisplayCodeSequence(4);
                    break;
                case "Five":
                    codeSequence += "5";
                    DisplayCodeSequence(5);
                    break;
                case "Six":
                    codeSequence += "6";
                    DisplayCodeSequence(6);
                    break;
                case "Seven":
                    codeSequence += "7";
                    DisplayCodeSequence(7);
                    break;
                case "Eight":                 
                    codeSequence += "8";
                    DisplayCodeSequence(8);
                    break;
                case "Nine":
                    codeSequence += "9";
                    DisplayCodeSequence(9);
                    break;
            }
        }

        switch (digit)
        {
            case "Star":
                ResetCodeSequence();
                break;
            case "Hash":
                if (codeSequence.Length > 0)
                {
                    CheckCodeSequence();
                }
                break;
        }
    }

    private void DisplayCodeSequence(int digit)
    {
        switch (codeSequence.Length)
        {
            case 1:
                characters[0].sprite = digits[10];
                characters[1].sprite = digits[10];
                characters[2].sprite = digits[10];
                characters[3].sprite = digits[digit];
                break;
            case 2:
                characters[0].sprite = digits[10];
                characters[1].sprite = digits[10];
                characters[2].sprite = characters[3].sprite;
                characters[3].sprite = digits[digit];
                break;
            case 3:
                characters[0].sprite = digits[10];
                characters[1].sprite = characters[2].sprite;
                characters[2].sprite = characters[3].sprite;
                characters[3].sprite = digits[digit];
                break;
            case 4:
                characters[0].sprite = characters[1].sprite;
                characters[1].sprite = characters[2].sprite;
                characters[2].sprite = characters[3].sprite;
                characters[3].sprite = digits[digit];
                break;
        }
    }

    private void ResetCodeSequence()
    {
        codeSequence = "";

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].sprite = digits[10];
        }

        denySound.Play();
    }

    private void CheckCodeSequence()
    {
        if (codeSequence == codeToUnlock)
        {
            Debug.Log("Correct code sequence");
            acceptSound.Play();
            CloseInterface();
            if(transform.parent.tag == "KeyPadPanelMaintenence"){
                sceneManager.GetComponent<OfficeFloor1SceneManager>().KeypadCorrectSequence("Maintenence");
            } else if (transform.parent.tag == "KeyPadLockerT3"){
                sceneManager.GetComponent<OfficeFloor1SceneManager>().KeypadCorrectSequence("LockerT3");
            } else if (transform.parent.tag == "KeyPadLocker12"){
                sceneManager.GetComponent<OfficeFloor2SceneManager>().KeypadCorrectSequence("Locker12");
            } else if (transform.parent.tag == "KeyPadLocker13"){
                sceneManager.GetComponent<OfficeFloor2SceneManager>().KeypadCorrectSequence("Locker13");
            } else if (transform.parent.tag == "KeyPadSecurityRoom"){
                sceneManager.GetComponent<DataCenterSceneManager>().KeypadCorrectSequence("SecRoom");
            }
        }
        else
        {
            Debug.Log("Incorrect code sequence");
            denySound.Play();
            ResetCodeSequence();
        }
    }

    private void OnDestroy()
    {
        PushButtonKeyPad.OnButtonPressed -= AddDigitToCodeSequence;
    }

    private void CloseInterface()
    {
        codeSequence = "";
        uiInterface.SetActive(false);

        PlayerMovement.SetCurrentInteractable(null);
        PlayerMovement.UnfreezePlayer();
    }


}
