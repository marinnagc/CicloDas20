using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alarmLightRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;
    // Update is called once per frame
    void Update()
    {
        // rotate 360 degrees over 2 seconds
        transform.rotation = Quaternion.Euler(0, 0, Time.time % 360 * rotationSpeed);
    }
}
