using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New PostIt Item", menuName = "Inventory/PostIt")]
public class PostItItem : Item
{
    public string PanelTag = "PostItModel";
    public string postItName = "New PostIt";
    public string pageOne = "Page One";

    public override void PickUp()
    {
        base.PickUp();

        Use();
    }

    public override void Use()
    {
        base.Use();

        Debug.Log("[PostIt Item Type] " + postItName);

        GameObject PostItWindowPanel = GameObject.FindGameObjectWithTag(PanelTag);
        if (PostItWindowPanel != null)
        {
            PostItWindowPanel.GetComponent<PostItWindowPanel>().SetSprite(base.icon);
            PostItWindowPanel.GetComponent<PostItWindowPanel>().SetPostIt(pageOne);
            PostItWindowPanel.GetComponent<PostItWindowPanel>().Open();
        }
        else
        {
            Debug.Log("PostItWindowPanel not found!");
        }
        
        
    }

}
