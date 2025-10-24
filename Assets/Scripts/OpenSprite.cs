using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSprite : Interactable
{
    private GameObject uiInterface;
    public string spriteNameTag;
    
    void Start()
    {
        uiInterface = GameObject.FindGameObjectWithTag(spriteNameTag);

        if (uiInterface == null)
        {
            Debug.LogError("OpenSprite: uiInterface not found!");
        }

        uiInterface.SetActive(false);
    }

    public override void Interact()
    {
        Debug.Log("Interacting with OpenSprite");
        
        PlayerMovement.FreezePlayer();
        uiInterface.SetActive(true);  

        PlayerMovement.SetCurrentInteractable(uiInterface);
    }
}
