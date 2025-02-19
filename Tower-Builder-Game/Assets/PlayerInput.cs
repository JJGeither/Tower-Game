using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode placeKey = KeyCode.F;

    void Update()
    {
        // Check for interact key press
        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }

        // Check for jump key press
        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        // Check for jump key press
        if (Input.GetKeyDown(placeKey))
        {
            Place();
        }
    }

    void Interact()
    {
        Debug.Log("Interacted with object!");
    }

    void Jump()
    {
        Debug.Log("Jumping!");
    }

    void Place()
    {

    }
}
