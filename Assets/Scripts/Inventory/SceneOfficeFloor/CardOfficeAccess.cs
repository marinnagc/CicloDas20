using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New CardItem Item", menuName = "Inventory/CardOfficeAccess")]
public class CardOfficeAccess : Item
{
    public override void PickUp()
    {
        base.PickUp();

        string[] sentences = new string[3];
        sentences[0] = "Olha! Uma destas caixas estava aberta!";
        sentences[1] = "Alguem deve ter deixado as coisas quando saiu correndo, esqueceu aqui o cartão, assim como esqueci o meu lá dentro";
        sentences[2] = "Será que isso me dá acesso à catraca?";

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
        sentences[0] = "Cartão de acesso com liberação baixa.";

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
