using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public GameObject inventoryUI;

    void Start()
    {
        Debug.Log("Inventory Start");
        if (inventoryUI == null)
        {
            Debug.Log("InventoryUI not found!");

            GameObject inventoryUIObject = GameObject.FindGameObjectWithTag("InventoryUI");

            if (inventoryUIObject != null)
            {
                inventoryUI = inventoryUIObject;
            }
            else
            {
                Debug.Log("InventoryUI not found!");
            }

            
        }
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    
    public int space = 20;
    
    public List<Item> items = new List<Item>();

    public List<Item> GetItems()
    {
        return items;
    }

    public bool Add(Item item)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough space.");
                return false;
            }

            if (Contains(item.name))
            {
                Debug.Log("Already have " + item.name);
                return false;
            }
            
            items.Add(item);

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        }
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    // check if inventory contains item by name
    public bool Contains(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.name == itemName)
            {
                return true;
            }
        }
        return false;
    }



}
