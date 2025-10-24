using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenterSceneManager : MonoBehaviour
{
    public GameObject TriggerDoorKeyPad, DoorSec, DoorSecOpened;
    void Start()
    {
        
    }

    public void KeypadCorrectSequence(string door){
        if (door == "SecRoom"){
            DoorSecOpened.SetActive(true);
            DoorSec.SetActive(false);
            TriggerDoorKeyPad.SetActive(false);
        }
    }
}
