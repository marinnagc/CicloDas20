using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostItWindowPanel : MonoBehaviour
{
    public GameObject postItWindowPanel;
    public AudioSource postItSound;

    void Start()
    {
        postItWindowPanel.SetActive(false);
    }

    [Header("PostIt")]
    [SerializeField]
    private TextMeshProUGUI pageOne;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Image PostitImage;


    public void SetPostIt(string pageOneText)
    {
        pageOne.text = pageOneText;
    }

    public void SetSprite(Sprite postit_sprite){
        PostitImage.sprite = postit_sprite;
    }

    public void Close()
    {
        postItSound.Play();
        
        postItWindowPanel.SetActive(false);
        PlayerMovement.SetCurrentInteractable(null);
        PlayerMovement.UnfreezePlayer();
    }

    public void Open()
    {
        postItSound.Play();

        postItWindowPanel.SetActive(true);
        PlayerMovement.SetCurrentInteractable(postItWindowPanel);
        PlayerMovement.FreezePlayer();
    }
}
