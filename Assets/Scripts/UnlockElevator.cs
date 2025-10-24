using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockElevator : Interactable
{
    public GameObject elevatorTrigger;
    public Collider2D elevatorUnlockCollider;
    public Item KeyItemToEleavator;
    private GameObject GFS;

    public override void Interact()
    {
        if (Inventory.instance.Contains(KeyItemToEleavator.name)){
            GFS = GameObject.FindGameObjectWithTag("GlobalFlagSystem");
            if (GFS == null)
            {
                Debug.LogError("OpenSprite: GlobalFlagSystem not found!");
            }
            GFS.GetComponent<GlobalFlagSystem>().elevatorUnlocked = true;
            elevatorTrigger.SetActive(true);
            elevatorUnlockCollider.enabled = false;
        }
    }
}