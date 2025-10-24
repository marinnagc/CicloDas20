using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New KeyItem Item", menuName = "Inventory/Key")]
public class KeyItem : Item
{

    // pickup
    public override void PickUp()
    {
        base.PickUp();

        Use();

    }

    public override void Use()
    {
        Debug.Log("[KeyItem Item Type] " + name);


        // load scene 1
        SceneManager.LoadScene(1);
    }

}
