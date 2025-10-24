using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseCodeLightBlink : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D light_source;
    public List<int> morse_code;

    private int index = 0;
    private float timer = 0f;

    private bool in_pause_time = false;
    
    // Start is called before the first frame update
    void Start()
    {
        light_source.intensity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime*1.5f;
        Debug.Log(timer);
        if (timer > morse_code[index] && !in_pause_time){
            if (light_source.intensity == 0f){
                light_source.intensity = 4f;
            } else {
                light_source.intensity = 0f;
            }
            timer = 0f;
            if (light_source.intensity > 0f){
                index++;
                if (index >= morse_code.Count){
                    in_pause_time = true;
                    index = 0;
                }
            }
        }
        if (in_pause_time){
            if (timer > 4f){
                in_pause_time = false;
                timer = 0f;
            }
        }
    }
}
