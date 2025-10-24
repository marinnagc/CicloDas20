using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLayerRender : MonoBehaviour
{
    public Transform playerPos;


    // Start is called before the first frame update
    void Start()
    {
        // define sprite sorting layer to BehindPlayer
        GetComponent<SpriteRenderer>().sortingLayerName = "BehindPlayer";
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Set sprite sorting layer to InFrontPlayer if pivot y position is lower than player y position
        if (transform.position.y < playerPos.position.y-0.7)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "InFrontPlayer";
        } else
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "BehindPlayer";
        }
    }
}
