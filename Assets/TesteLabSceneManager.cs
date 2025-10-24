using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesteLabSceneManager : MonoBehaviour
{
    public GameObject TriggerYellowDrawer, TriggerYellowDrawerOpened;
    // Start is called before the first frame update
    public void LockCorrectSequence(string lock_name){
        if (lock_name == "YellowDrawer"){
            TriggerYellowDrawer.SetActive(false);
            TriggerYellowDrawerOpened.SetActive(true);
        }
    }
}
