using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New KeyItem Item", menuName = "Inventory/KeyItemToElevatorOn")]
public class KeyItemToElevatorOn : Item
{

    // pickup
    public override void PickUp()
    {
        base.PickUp();
        string[] sentences = new string[2];
        sentences[0] = "Acho que esta mala amarela é do Pablo...";
        sentences[1] = "Ele guardou aqui uma chave, o que será que ela abre?";

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
        sentences[0] = "Chave encontrada na mala do Pablo.";

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
