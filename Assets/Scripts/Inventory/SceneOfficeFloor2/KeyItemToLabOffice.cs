using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New KeyItem Item", menuName = "Inventory/KeyItemToLabOffice")]
public class KeyItemToLabOffice : Item
{

    // pickup
    public override void PickUp()
    {
        base.PickUp();
        string[] sentences = new string[1];
        sentences[0] = "Uma chave Nova! O que será que consigo abrir com ela?";

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
        sentences[0] = "Chave para o Laboratório de Testes Químicos.";

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
