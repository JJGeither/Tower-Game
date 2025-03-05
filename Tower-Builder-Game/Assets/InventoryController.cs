using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    public List<GameObject> Inventory = new List<GameObject>();
    public float rotationSpeed = 5f; // Adjust sensitivity

    private float wheelPosX = 0f; // Track rotation for X-axis
    private float wheelPosY = 0f; // Track rotation for Y-axis
    int rotationValue = 0;

    private void Update()
    {
        UpdateInventoryPosition(ReticleInteractionHandler.Instance.rayFromReticle);
    }

    private Vector3 lastHitPoint = Vector3.zero;
    private Vector3 lastHitNormal = Vector3.up; // Default normal facing up

    public void UpdateInventoryPosition(Ray ray)
    {
        if (Inventory.Count > 0)
        {
            GameObject item = Inventory[0];
            Collider itemCollider = item.GetComponentInChildren<Collider>();

            if (itemCollider != null)
            {
                itemCollider.enabled = false;
            }

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                lastHitPoint = hitInfo.point;
                lastHitNormal = hitInfo.normal;

                Vector3 hitPoint = hitInfo.point;
                Vector3 hitNormal = hitInfo.normal;

                Vector3 horizontalDirection = new Vector3(hitPoint.x - PlayerComponentManager.Instance.transform.position.x, 0, hitPoint.z - PlayerComponentManager.Instance.transform.position.z);
                float distanceToPlayer = horizontalDirection.magnitude;

                if (distanceToPlayer < ReticleInteractionHandler.Instance.minimumDistance)
                {
                    horizontalDirection = horizontalDirection.normalized * ReticleInteractionHandler.Instance.minimumDistance;
                }

                item.transform.position = new Vector3(
                    PlayerComponentManager.Instance.transform.position.x + horizontalDirection.x,
                    hitPoint.y,
                    PlayerComponentManager.Instance.transform.position.z + horizontalDirection.z
                );

                // Apply rotation to the parent (look at the target)
                Quaternion parentRotation = GetYLookRotation(PlayerComponentManager.Instance.transform.position, item.transform.position);
                item.transform.rotation = parentRotation; // Apply rotation to the parent only

            }

            RotateWithScrollWheel(ref item, ref rotationValue);


            Vector3 resultVector;
            int resultRotation;
            GetVectorAndRotation(item, rotationValue, out resultVector, out resultRotation);

            // Set the child’s position based on calculated offset
            item.transform.GetChild(0).transform.localPosition = resultVector;

            // Set the child’s rotation with zero Y rotation, making it ignore parent rotation
            Quaternion childRotation = Quaternion.Euler(resultRotation, 0, 0); // Zero out Y rotation
            item.transform.GetChild(0).transform.localRotation = childRotation; // Apply only the necessary rotation to the child (no Y rotation)

        }
    }

    public static Quaternion GetYLookRotation(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        direction.y = 0; // Ignore Y-axis changes
        if (direction != Vector3.zero)
        {
            return Quaternion.LookRotation(direction);
        }
        return Quaternion.identity; // Default rotation if no direction
    }

    private void RotateWithScrollWheel(ref GameObject item, ref int rotationValue)
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // Scroll up
        {
            rotationValue += 1;
        }
        else if (scroll < 0f) // Scroll down
        {
            rotationValue -= 1;
        }
    }

    public void GetVectorAndRotation(GameObject item, int n, out Vector3 vector, out int rotation)
    {
        Vector3 t = item.transform.GetChild(0).GetComponent<BoxCollider>().size;

        // Cycle the index to one of the four states
        int cycleIndex = n % 4;

        // Initialize output parameters
        vector = Vector3.zero;
        rotation = 0;

        // Get the local scale (replace `transform.localScale` with the actual object scale if necessary)
        Vector3 localScale = item.transform.GetChild(0).transform.localScale; // Adjust this if needed to get a specific scale

        // Return the corresponding vector and rotation based on the cycle index
        switch (cycleIndex)
        {
            case 0:
                vector = new Vector3(0, 0, 0);
                rotation = 0;
                break;
            case 1:
                // Multiply by local scale
                vector = new Vector3(0, (t.z / 2) * localScale.y, -(t.x / 2) * localScale.x);
                rotation = 90;
                break;
            case 2:
                // Multiply by local scale
                vector = new Vector3(0, t.y * localScale.y, 0);
                rotation = 180;
                break;
            case 3:
                // Multiply by local scale
                vector = new Vector3(0, (t.z / 2) * localScale.y, (t.x / 2) * localScale.x);
                rotation = 270;
                break;
        }
    }

    // Draw a gizmo to visualize the normal
    private void OnDrawGizmos()
    {
        if (lastHitPoint != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastHitPoint, lastHitPoint + lastHitNormal * 5f); // Draw normal
        }
    }

    public void AddToInventory(GameObject item)
    {
        if (item != null)
        {
            Inventory.Add(item);
            Debug.Log("Item added to inventory: " + item.name);
            ShowInventory();
        }
        else
        {
            Debug.LogWarning("Attempted to add a null item to the inventory.");
        }
    }

    public void RemoveFromInventory(GameObject item)
    {
        if (Inventory.Contains(item))
        {
            Inventory.Remove(item);
            Debug.Log("Item removed from inventory: " + item.name);
        }
        else
        {
            Debug.LogWarning("Item not found in inventory: " + item.name);
        }
    }

    public void PlaceItem()
    {
        if (Inventory.Count > 0 && Inventory[0] != null)
        {
            GameObject item = Inventory[0];
            Inventory.Remove(item);

            // Reactivate the collider before placing the item
            BoxCollider itemCollider = item.transform.GetChild(0).GetComponent<BoxCollider>();
            if (itemCollider != null)
            {
                itemCollider.enabled = true; // Reactivate the collider
            }
        }
    }

    public void ShowInventory()
    {
        if (Inventory.Count > 0)
        {
            Debug.Log("Current Inventory:");
            foreach (var item in Inventory)
            {
                Debug.Log("- " + item.name);
            }
        }
        else
        {
            Debug.Log("Inventory is empty.");
        }
    }
}
