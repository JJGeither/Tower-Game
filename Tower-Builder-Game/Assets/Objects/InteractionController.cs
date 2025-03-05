using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private Interactable interactableInstance;
    public GameObject powerupPrefab;

    public enum InteractionType
    {
        Default,
        Pickup,
        Chest
    }

    [SerializeField]
    private InteractionType interactionType;

    void Start()
    {
        InitializeInteractable();
    }

    private void InitializeInteractable()
    {
        switch (interactionType)
        {
            case InteractionType.Default:
                interactableInstance = new Interactable();
                break;
            case InteractionType.Pickup:
                interactableInstance = new Pickup(gameObject);
                break;
            case InteractionType.Chest:
                interactableInstance = new Chest(gameObject, powerupPrefab);
                break;

            default:
                interactableInstance = new Interactable();
                break;
        }
    }

    public void Interact()
    {
        interactableInstance.Interact();
    }

    public class Interactable
    {
        public virtual void Interact()
        {
            Debug.Log("Interacting with base interactable.");
        }
    }

    public class Chest : Interactable
    {
        private bool isOpen = false;
        private GameObject powerupInstance;
        private GameObject ownerObject;

        public Chest(GameObject ownerObject, GameObject powerupPrefab)
        {
            this.ownerObject = ownerObject;
            this.powerupInstance = powerupPrefab;
        }

        public override void Interact()
        {
            if (!isOpen)
            {
                isOpen = true;
                Debug.Log("Opening chest and spawning powerup.");
                Instantiate(powerupInstance, ownerObject.transform);
            }
        }
    }

    public class Pickup : Interactable
    {
        private GameObject ownerObject;

        public Pickup(GameObject ownerObject)
        {
            this.ownerObject = ownerObject;
        }

        public override void Interact()
        {
            Debug.Log("Picking up item.");
            PlayerComponentManager.Instance.AddToInventory(this.ownerObject);
        }
    }


    // Function to draw the normal of a face
    void DrawFaceNormal(Vector3 center, Vector3 halfExtents, Vector3 direction)
    {
        // Calculate the midpoint of the face
        Vector3 faceCenter = center + Vector3.Scale(halfExtents, direction);

        // Draw the normal using Gizmos
        Gizmos.color = Color.red;  // Normal color (red)
        Gizmos.DrawLine(faceCenter, faceCenter + direction * 1.0f);  // Draw line representing normal
    }


    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = this.GetComponentInChildren<BoxCollider>();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            // Get the center and size of the box collider
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            // Transform the boxCollider's local space to world space
            Vector3 worldCenter = transform.TransformPoint(center);
            Vector3 halfExtents = size * 0.5f;

            // Draw the normals for each face of the box
            DrawFaceNormal(worldCenter, halfExtents, Vector3.forward);  // Front face
            DrawFaceNormal(worldCenter, halfExtents, Vector3.back);     // Back face
            DrawFaceNormal(worldCenter, halfExtents, Vector3.left);     // Left face
            DrawFaceNormal(worldCenter, halfExtents, Vector3.right);    // Right face
            DrawFaceNormal(worldCenter, halfExtents, Vector3.up);       // Top face
            DrawFaceNormal(worldCenter, halfExtents, Vector3.down);     // Bottom face
        }
    }
}
