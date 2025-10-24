using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockerDisplay : MonoBehaviour
{

    public PushButtonLocker[] pushButtonLockers;

    public string[] correctCode;

    public Button tryButton;

    public Image lockerDoor;

    public GameObject lockerInterface;

    public AudioSource acceptSound;
    public AudioSource denySound;

    public GameObject SceneManager;


    void Start()
    {
        // subscribe to event OnButtonPressed

        tryButton.onClick.AddListener(CheckCode);
    }

    private void CheckCode()
    {
        // check if all 4 characters are set
        if (pushButtonLockers[0].GetCurrentCharacter() != "" && pushButtonLockers[1].GetCurrentCharacter() != "" && pushButtonLockers[2].GetCurrentCharacter() != "" && pushButtonLockers[3].GetCurrentCharacter() != "")
        {
            // check if all 4 characters are correct
            if (pushButtonLockers[0].GetCurrentCharacter() == correctCode[0] && pushButtonLockers[1].GetCurrentCharacter() == correctCode[1] && pushButtonLockers[2].GetCurrentCharacter() == correctCode[2] && pushButtonLockers[3].GetCurrentCharacter() == correctCode[3])
            {
                lockerDoor.enabled = false;
                acceptSound.Play();
                if (transform.parent.tag == "LockBagStorage") SceneManager.GetComponent<OfficeFloor1SceneManager>().LockCorrectSequence("StorageBag");
                if (transform.parent.tag == "LockTesteLab") SceneManager.GetComponent<TesteLabSceneManager>().LockCorrectSequence("YellowDrawer");

                CloseInterface();
            } else {
                denySound.Play();
            }
        }
    }

    public void CloseInterface()
    {
        lockerInterface.SetActive(false);

        PlayerMovement.SetCurrentInteractable(null);
        PlayerMovement.UnfreezePlayer();
    }
}
