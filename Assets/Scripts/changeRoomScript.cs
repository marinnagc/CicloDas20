using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeRoomScript : MonoBehaviour
{
    public Vector2 warpPosition;
    public Vector2 nextCameraMinPos;
    public Vector2 nextCameraMaxPos;
    public GameObject SceneManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFeet"))
        {
            GameObject player = collision.transform.parent.gameObject;
            player.GetComponent<PlayerMovement>().enabled = false;
            SceneManager.GetComponent<OfficeFloor1SceneManager>().changeRoom(warpPosition, nextCameraMinPos, nextCameraMaxPos);
        }
    }
}
