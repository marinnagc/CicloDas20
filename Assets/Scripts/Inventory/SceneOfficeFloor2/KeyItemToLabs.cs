using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New KeyItem Item", menuName = "Inventory/KeyItemToLabs")]
public class KeyItemToLabs : Item
{

    // pickup
    public override void PickUp()
    {
        base.PickUp();
        string[] sentences = new string[1];
        sentences[0] = "Deve ser a chave de acesso para os outros laboratórios...";

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
        sentences[0] = "Chave para os Laboratório no S2.";

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
