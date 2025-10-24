using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New PenDriveItem Item", menuName = "Inventory/PenDrive")]
public class PenDriveItem : Item
{
    public override void PickUp()
    {
        base.PickUp();
        string[] sentences = new string[2];
        sentences[0] = "Que estranho, uma mala amarela de novo?";
        sentences[1] = "Nela tem um Pendrive com um adesivo escrito 'malware.exe'. Será que é um vírus?";

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

    public override void Use()
    {
        string[] sentences = new string[1];
        sentences[0] = "Um Pendrive com um arquivo de texto chamado 'malware.exe'.";

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
