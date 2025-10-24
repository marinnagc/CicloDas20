using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Singleton
    public static PlayerMovement instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerMovement found!");
            return;
        }
        instance = this;
        interactIcon.SetActive(false);
        flashlight.SetActive(flashlight_on);
    }
    #endregion
    
    private static GameObject currentInteractable;

    public GameObject interactIcon;

    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Animator animator;
    public Animator bodyAnimator;


    public Collider2D interactCollider;

    public GameObject flashlight;
    private static bool flashlight_on = false;

    private Vector2 movement;
    private bool isMoving = false;
    public AudioSource audioSourceSteps;

    public static void SetCurrentInteractable(GameObject interactable)
    {
        currentInteractable = interactable;
    }

    // void Awake()
    // {
    //     interactIcon.SetActive(false);
    //     flashlight.SetActive(flashlight_on);
    // }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Inventory.instance.Contains("FlashLight"))
            {
                SwitchFlashlight();
            }
            else {
                Debug.Log("You don't have a flashlight!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && currentInteractable != null)
        {
            currentInteractable.SetActive(false);
            UnfreezePlayer();
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        bodyAnimator.SetFloat("Horizontal", movement.x);
        bodyAnimator.SetFloat("Vertical", movement.y);
        bodyAnimator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x != 0)
        {
            isMoving = true;
            interactCollider.offset = new Vector2(movement.x, -0.5f);
            flashlight.transform.rotation = Quaternion.Euler(0, 0, 180 + (movement.x*90));
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", 0);

            bodyAnimator.SetFloat("LastHorizontal", movement.x);
            bodyAnimator.SetFloat("LastVertical", 0);
        }
        else if (movement.y != 0)
        {
            isMoving = true;
            flashlight.transform.rotation = Quaternion.Euler(0, 0, 90 - (movement.y*90));
            interactCollider.offset = new Vector2(0, movement.y - 0.5f);
            animator.SetFloat("LastHorizontal", 0);
            animator.SetFloat("LastVertical", movement.y);

            bodyAnimator.SetFloat("LastHorizontal", 0);
            bodyAnimator.SetFloat("LastVertical", movement.y);
        }
        else{
            isMoving = false;
        }

        if (isMoving){
            if (!audioSourceSteps.isPlaying) {
                audioSourceSteps.Play();
            }
        } else {
            audioSourceSteps.Stop();
        }


    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void OpenInteractIcon()
    {
        interactIcon.SetActive(true);
    }

    public void CloseInteractIcon()
    {
        interactIcon.SetActive(false);
    }

    public static void FreezePlayer()
    {
        GameObject.Find("Player").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public static void UnfreezePlayer()
    {
        GameObject.Find("Player").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GameObject.Find("Player").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void CheckInteraction()
    {
        Vector2 direction = new Vector2(animator.GetFloat("LastHorizontal"), animator.GetFloat("LastVertical"));
        // show debug raycast beam.
        Debug.Log(transform.position);
        Vector3 pActPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (animator.GetFloat("LastVertical") < 0){
            Debug.Log("lockingDown");
            pActPos = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }
        Debug.DrawRay(pActPos, direction, Color.red, 0.8f);

        RaycastHit2D hit = Physics2D.Raycast(pActPos, direction, 0.8f, LayerMask.GetMask("Trigger Raycast"));
        if (hit)
        {
            Debug.Log("Interacting with " + hit.collider.name);
            if (hit.transform.GetComponent<Interactable>())
            {
                hit.collider.GetComponent<Interactable>().Interact();
            }
        }
    }

    public void SwitchFlashlight()
    {
        flashlight.SetActive(!flashlight_on);
        flashlight_on = !flashlight_on;
    }
}
