using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New FlashLight Item", menuName = "Inventory/FlashLight")]
public class FlashLightItem : Item
{

    public override void PickUp()
    {
        base.PickUp();

        string[] sentences = new string[2];
        sentences[0] = "Uma Lanterna! Posso utilizá-la para iluminar lugares escuros";
        sentences[1] = "Posso ligá-la precionando F ou clicando no inventário.";

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
        PlayerMovement.instance.SwitchFlashlight();    
    }

}
