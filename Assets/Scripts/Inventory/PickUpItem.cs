using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : Interactable
{
    public Item item;

    public override void Interact()
    {
        Debug.Log("Interacting with PickUpItem");
        PickUp();
    }

    void PickUp()
    {

        bool wasPickedUp = Inventory.instance.Add(item);
        if (wasPickedUp)
        {
            item.PickUp();
            Debug.Log("Picked up " + item.name);
        }

    }
}
