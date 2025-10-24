using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New CardItem Item", menuName = "Inventory/CardTestLabAccess")]
public class CardTestLabAccess : Item
{
    public override void PickUp()
    {
        base.PickUp();

        string[] sentences = new string[2];
        sentences[0] = "Olha! Uma novo cartão de acesso!";
        sentences[1] = "Este me permite acessar o laboratório de testes químicos no S2.";

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
        sentences[0] = "Cartão de acesso para o laboratório no S2.";

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
