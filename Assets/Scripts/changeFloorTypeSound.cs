using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeFloorTypeSound : MonoBehaviour
{
    public AudioClip materialAudio;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFeet") )
        {
            collision.GetComponent<AudioSource>().clip = materialAudio;
            collision.GetComponent<AudioSource>().volume = 0.5f;
        }
    }
}
