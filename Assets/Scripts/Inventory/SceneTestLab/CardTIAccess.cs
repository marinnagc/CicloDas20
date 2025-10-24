using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New CardItem Item", menuName = "Inventory/CardTIAccess")]
public class CardTIAccess : Item
{
    public override void PickUp()
    {
        base.PickUp();

        string[] sentences = new string[2];
        sentences[0] = "O Cartão Azul é do pessoal do TI!";
        sentences[1] = "Isso deve me dar acesso a mais informações.";

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
        sentences[0] = "Cartão de acesso com liberação de TI.";

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
