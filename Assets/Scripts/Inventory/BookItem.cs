using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New Book Item", menuName = "Inventory/Book")]
public class BookItem : Item
{
    public string bookName = "New Book";
    public string pageOne = "Page One";
    public string pageTwo = "Page Two";

    public override void PickUp()
    {
        base.PickUp();

        Use();
    }

    public override void Use()
    {
        base.Use();

        Debug.Log("[Book Item Type] " + bookName);

        GameObject bookWindowPanel = GameObject.FindGameObjectWithTag("BookModel");
        if (bookWindowPanel != null)
        {
            bookWindowPanel.GetComponent<BookWindowPanel>().SetBook(pageOne, pageTwo);
            bookWindowPanel.GetComponent<BookWindowPanel>().Open();
        }
        else
        {
            Debug.Log("BookWindowPanel not found!");
        }
        
        
    }

}
