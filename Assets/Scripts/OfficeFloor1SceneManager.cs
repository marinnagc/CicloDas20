using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeFloor1SceneManager : MonoBehaviour
{
    public GameObject player;
    public GameObject playerBody;
    public GameObject playerFeet;
    public GameObject camera;

    public GameObject MaintenenceDoorOpen, MaintenenceDoorClosed;
    public Collider2D DoorTrigger;
    public Animator changePlayerRoomAnim;
    private Vector2 warpPosition;
    private Vector2 cameraMinPos;
    private Vector2 cameraMaxPos;
    private bool changeRoomActive = false;
    private bool FadeIn = false;
    private bool FadeOut = false;
    public GameObject ElevatorTrigger;
    public GameObject UnlockElevatorTrigger;
    private GameObject GFS;

    public GameObject TriggerLockerKeyPad, TriggerLockerOpened;
    public GameObject TriggerLockStorageBag, TriggerStorageBagOpened;

    public void Start(){
        GFS = GameObject.FindGameObjectWithTag("GlobalFlagSystem");

        if (GFS == null)
        {
            Debug.LogError("OpenSprite: GlobalFlagSystem not found!");
        }
        if (GFS.GetComponent<GlobalFlagSystem>().gameStarted){
            player.GetComponent<Rigidbody2D>().position = new Vector2(11f, 7.2f);
            camera.GetComponent<cameraScript>().minPosition = new Vector2(-3.2f, -3.4f);
            camera.GetComponent<cameraScript>().maxPosition = new Vector2(3.6f, 4.15f);
        }

        GFS.GetComponent<GlobalFlagSystem>().gameStarted = true;
        if (GFS.GetComponent<GlobalFlagSystem>().elevatorUnlocked){
            ElevatorTrigger.SetActive(true);
            UnlockElevatorTrigger.SetActive(false);
        } else {
            ElevatorTrigger.SetActive(false);
        }
    }

    public void Update(){
        if (changeRoomActive) {
            if(FadeIn){
                playerFeet.GetComponent<AudioSource>().Stop();
                player.GetComponent<Animator>().SetFloat("Speed", 0f);
                playerBody.GetComponent<Animator>().SetFloat("Speed", 0f);
                changePlayerRoomAnim.Play("FadeIn");
                FadeIn = false;
                FadeOut = true;
            }
            else if(!changePlayerRoomAnim.GetCurrentAnimatorStateInfo(0).IsName("FadeIn") && FadeOut){
                player.GetComponent<Rigidbody2D>().position = warpPosition;
                camera.GetComponent<cameraScript>().maxPosition = cameraMaxPos;
                camera.GetComponent<cameraScript>().minPosition = cameraMinPos;
                changePlayerRoomAnim.Play("FadeOut");
                FadeOut = false;
            }
            else if(!changePlayerRoomAnim.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") && !FadeOut && !FadeIn){
                player.GetComponent<PlayerMovement>().enabled = true;
                changeRoomActive = false;
            }
        }
    }

    public void changeRoom(Vector2 DesireWarpPosition, Vector2 CameraMinPos, Vector2 CameraMaxPos){
        warpPosition = DesireWarpPosition;
        cameraMinPos = CameraMinPos;
        cameraMaxPos = CameraMaxPos;
        changeRoomActive = true;
        FadeIn = true;
    }

    public void KeypadCorrectSequence(string door){
        if (door == "Maintenence"){
            MaintenenceDoorClosed.SetActive(false);
            DoorTrigger.enabled = false;
            MaintenenceDoorOpen.SetActive(true);
        } else if (door == "LockerT3"){
            TriggerLockerOpened.SetActive(true);
            TriggerLockerKeyPad.SetActive(false);
            string[] sentences = new string[1];
            sentences[0] = "Opa, a senha estava certa! O que será que tem dentro?";

            string name = "EU";

            GameObject dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
            if (dialogueBox != null)
            {
                Debug.Log("DialogueBox found!");
                dialogueBox.GetComponent<DialogueScript>().SetNewDialogue(sentences, name);
            }
            else
            {
                Debug.Log("DialogueBox not found!");
            }
        }
    }
    public void LockCorrectSequence(string lock_name){
        if (lock_name == "StorageBag"){
            TriggerLockStorageBag.SetActive(false);
            TriggerStorageBagOpened.SetActive(true);
        }
    }

}
