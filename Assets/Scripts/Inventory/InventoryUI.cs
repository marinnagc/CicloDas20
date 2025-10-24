using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    Inventory inventory;

    InventorySlot[] slots;

    void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("InventoryUI Start");
        
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        UpdateUI();

        inventoryUI.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "MainMenuReset" )
        {
            inventory.items.Clear();
            inventoryUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "MainMenuReset")
            {
                Debug.Log("InventoryUI: MainMenu");
                return;
            }

            Debug.Log("Toggling inventory");
    
            UpdateUI();

            inventoryUI.SetActive(!inventoryUI.activeSelf);

            if (inventoryUI.activeSelf)
                inventoryUI.LeanScale(Vector3.one, 0.25f).setEaseInOutExpo();
            else
                inventoryUI.LeanScale(Vector3.zero, 0.25f).setEaseInOutExpo();
        }        
    }

    void UpdateUI()
    {
        Debug.Log("Updating UI");
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                Debug.Log("Adding item to slot " + i);
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                // Debug.Log("Clearing slot " + i);
                slots[i].ClearSlot();
            }
        }

    }
}
