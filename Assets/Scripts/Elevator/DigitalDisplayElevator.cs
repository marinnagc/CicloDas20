using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DigitalDisplayElevator : MonoBehaviour
{
    public GameObject uiInterface;
    public GameObject loadingScreen;

    [Header("Sounds")]
    [SerializeField]
    public AudioSource openSound;
    [SerializeField]
    public AudioSource closeSound;
    [SerializeField]
    public AudioSource alarmSound;

    [Header("Display Text")]
    [SerializeField]
    public TextMeshProUGUI currentFloorText;
    [SerializeField]
    public TextMeshProUGUI targetFloorText;
    [SerializeField]

    private Dictionary<string, string> buttonToSceneName = new Dictionary<string, string>()
    {
        {"S3", "BioLab"},
        {"S2", "TestLab"},
        {"S1", "DataCenter"},
        {"T", "OfficeFloor1"},
        {"1", "OfficeFloor2"},
        {"2", "SampleScene"},
    };

    private Dictionary<string, string> buttonToItemName = new Dictionary<string, string>()
    {
        {"S3", "CardBossAccess"},
        {"S2", "CardTestLabAccess"},
        {"S1", "CardTIAccess"},
        {"T", "CardOfficeAccess"},
        {"1", "CardOfficeAccess"},
        {"2", "Card3"},
    };

    private string GetCurrentFloorBySceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string currentFloor = "";

        if (sceneName == "BioLab")
        {
            currentFloor = "S3";
        }
        else if (sceneName == "TestLab")
        {
            currentFloor = "S2";
        }
        else if (sceneName == "DataCenter")
        {
            currentFloor = "S1";
        }
        else if (sceneName == "OfficeFloor1")
        {
            currentFloor = "T";
        }
        else if (sceneName == "OfficeFloor2")
        {
            currentFloor = "1";
        }
        else if (sceneName == "SampleScene")
        {
            currentFloor = "2";
        }

        return currentFloor;
    }

    private string currentFloor;
    private string targetFloor;

    #region Singleton
    public static DigitalDisplayElevator instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DigitalDisplayElevator found!");
            return;
        }

        instance = this;
    }
    #endregion

    void Start()
    {
        Debug.Log("DigitalDisplayElevator Start");
        PushButtonElevator.OnButtonPressed += UpdateTargetFloor;

        currentFloor = GetCurrentFloorBySceneName();
        targetFloor = currentFloor;

        currentFloorText.text = currentFloor;
        targetFloorText.text = targetFloor;
    }

    void Update()
    {
        // update current floor
        currentFloor = GetCurrentFloorBySceneName();
        currentFloorText.text = currentFloor;
    }

    private void UpdateTargetFloor(string buttonValue)
    {
        if (buttonValue == "GO")
        {
            if (currentFloor == targetFloor)
            {
                Debug.Log("Already on the floor");
                ElevatorMessage("Already on the floor");
                return;
            }
            else
            {
                // check if has item
                if (Inventory.instance.Contains(buttonToItemName[targetFloor]))
                {
                    Debug.Log("Going to the target floor");
                    StartCoroutine(GoToTargetFloor());
                }
                else
                {
                    Debug.Log("You don't have the key to go to the target floor");
                    ElevatorMessage("NO CARD PERMISSION");
                }
            }
        }
        else if (buttonValue == "ALARM")
        {
            Debug.Log("Alarm is pressed");
            StartCoroutine(Alarm());
        }
        else if (buttonValue == "CLEAR")
        {
            Debug.Log("Clear is pressed");
            StartCoroutine(Clear());
        }
        else
        {
            Debug.Log("Target floor is updated to: " + buttonValue);
            targetFloor = buttonValue;
            targetFloorText.text = targetFloor;
        }
    }

    private IEnumerator GoToTargetFloor()
    {
        // play open sound
        openSound.Play();

        // freeze player
        PlayerMovement.FreezePlayer();

        // show ui interface
        // loadingScreen.SetActive(true);

        // load scene
        SceneManager.LoadScene(buttonToSceneName[targetFloor]);

        // play close sound
        closeSound.Play();

        // wait for 3 seconds
        yield return new WaitForSeconds(3);

        // update current floor
        currentFloor = targetFloor;
        currentFloorText.text = currentFloor;
        
        // loadingScreen.SetActive(false);

        // close elevator ui
        // CloseElevatorUI();
    }

    private IEnumerator Alarm()
    {
        // wait for 3 seconds
        yield return new WaitForSeconds(3);
        // play open sound
        alarmSound.Play();
    }

    private IEnumerator Clear()
    {
        // play open sound
        openSound.Play();

        // freeze player
        PlayerMovement.FreezePlayer();

        // show ui interface
        uiInterface.SetActive(true);

        // wait for 3 seconds
        yield return new WaitForSeconds(3);

        CloseElevatorUI();
    }

    // insert message off error in next_floor value text
    private void ElevatorMessage(string message)
    {
        targetFloorText.text = message.ToString().ToUpper();
    }

    // private void OnDestroy()
    // {
    //     PushButtonElevator.OnButtonPressed -= UpdateTargetFloor;
    // }

    public void OpenElevatorUI()
    {
        currentFloor = GetCurrentFloorBySceneName();
        targetFloor = currentFloor;

        currentFloorText.text = currentFloor;
        targetFloorText.text = targetFloor;

        // play open sound
        openSound.Play();

        // freeze player
        PlayerMovement.FreezePlayer();

        // show ui interface
        uiInterface.SetActive(true);
    }

    // close elevator ui function
    public void CloseElevatorUI()
    {
        // play close sound
        closeSound.Play();

        // hide ui interface
        uiInterface.SetActive(false);
     
        // unfreeze player
        PlayerMovement.UnfreezePlayer();
        PlayerMovement.SetCurrentInteractable(null);
    }
}
