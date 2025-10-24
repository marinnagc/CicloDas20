using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New CardItem Item", menuName = "Inventory/Card")]
public class CardItem : Item
{

    public override void Use()
    {
        Debug.Log("[CardItem Item Type] " + name);
    }

}
