using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    public GameObject dialogueBox;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public AudioSource audioSource;
    // public Image portrait;

    private string[] sentences;
    private float textSpeed = 0.05f;

    private int index;

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    void Update()
    {

        if (((Input.GetMouseButtonDown(0) ||  ( Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) && dialogueBox.activeSelf))
        {
            if (dialogueText.text == sentences[index])
            {
                NextSentence();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = sentences[index];
            }
        }
        
    }

    public void SetNewDialogue(string[] newSentences, string newName)
    {
        sentences = newSentences;
        nameText.text = newName;
        StartDialogue();
    }

    void StartDialogue()
    {
        dialogueBox.SetActive(true);
        index = 0;
        StartCoroutine(TypeSentence(sentences[index]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        audioSource.Play();
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        audioSource.Stop();
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(TypeSentence(sentences[index]));
        }
        else
        {
            dialogueBox.SetActive(false);
            dialogueText.text = "";
            index = 0;
        }
    }
}
