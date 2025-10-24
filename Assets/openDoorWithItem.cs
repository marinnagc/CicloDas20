using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoorWithItem : Interactable
{
    public GameObject DoorLeftClosed, DoorRightClosed, DoorOpened;
    public Collider2D TriggerOpenDoor;
    public Item KeyItemToLabOffice;

    public override void Interact()
    {
        Debug.Log("Interagiu com a porta");
        if (Inventory.instance.Contains(KeyItemToLabOffice.name)){
            Debug.Log("Possui a chave");
            DoorLeftClosed.SetActive(false);
            DoorRightClosed.SetActive(false);
            DoorOpened.SetActive(true);
            TriggerOpenDoor.enabled = false;
        } else {
            string[] sentences = new string[1];
            sentences[0] = "Acho que não possua a chave para esta porta!";

            string name = "EU";

            GameObject dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
            if (dialogueBox != null)
            {
                Debug.Log("DialogueBox found!");
                dialogueBox.GetComponent<DialogueScript>().SetNewDialogue(sentences, name);
            }
            else
            {
                Debug.Log("DialogueBox not found!");
            }
        }
    }
}