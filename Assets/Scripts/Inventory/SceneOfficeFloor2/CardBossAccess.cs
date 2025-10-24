using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New CardItem Item", menuName = "Inventory/CardBossAccess")]
public class CardBossAccess : Item
{
    public override void PickUp()
    {
        base.PickUp();

        string[] sentences = new string[1];
        sentences[0] = "O cartão de acesso do Chefe!";

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
        sentences[0] = "Cartão de acesso do chefe do laboratório.";

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
