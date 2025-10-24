using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookWindowPanel : MonoBehaviour
{
    public GameObject bookWindowPanel;
    public AudioSource bookSound;

    void Start()
    {
        bookWindowPanel.SetActive(false);
    }

    [Header("Book")]
    [SerializeField]
    private TextMeshProUGUI pageOne;
    [SerializeField]
    private TextMeshProUGUI pageTwo;
    [SerializeField]
    private Button closeButton;


    public void SetBook(string pageOneText, string pageTwoText)
    {
        pageOne.text = pageOneText;
        pageTwo.text = pageTwoText;
    }

    public void Close()
    {
        bookSound.Play();
        
        bookWindowPanel.SetActive(false);
        PlayerMovement.SetCurrentInteractable(null);
        PlayerMovement.UnfreezePlayer();
    }

    public void Open()
    {
        bookSound.Play();

        bookWindowPanel.SetActive(true);
        PlayerMovement.SetCurrentInteractable(bookWindowPanel);
        PlayerMovement.FreezePlayer();
    }
}
