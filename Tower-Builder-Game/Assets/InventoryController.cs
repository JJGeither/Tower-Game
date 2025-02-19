using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    public List<GameObject> Inventory = new List<GameObject>();
    public float rotationSpeed = 5f; // Adjust sensitivity

    private float verticalRotation = 0f; // Track rotation angle

    private void Update()
    {
        UpdateInventoryPosition(ReticleInteractionHandler.Instance.rayFromReticle);
        RotateWithScrollWheel();
    }

    public void UpdateInventoryPosition(Ray ray)
    {
        if (Inventory.Count > 0)
        {
            GameObject item = Inventory[0];
            Collider itemCollider = item.GetComponent<Collider>();

            // Disable the collider so the item doesn't interfere with the spherecast.
            if (itemCollider != null)
            {
                itemCollider.enabled = false;
            }

            item.transform.position = ReticleInteractionHandler.Instance.ReticleHitPoint;
        }
    }


    private void RotateWithScrollWheel()
    {
        if (Inventory.Count > 0)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0) // Only rotate when scrolling
            {
                Vector3 rotationAxis = Input.GetMouseButton(1) ? Vector3.right : Vector3.up; // Right-click changes axis
                Inventory[0].transform.Rotate(rotationAxis, scrollInput * rotationSpeed * Time.deltaTime, Space.World);
            }
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

    public void PlaceItem(Vector3 point)
    {
        if (Inventory.Count > 0 && Inventory[0] != null)
        {
            GameObject item = Inventory[0];
            Inventory.Remove(item);

            // Reactivate the collider before placing the item
            Collider itemCollider = item.GetComponent<Collider>();
            if (itemCollider != null)
            {
                itemCollider.enabled = true; // Reactivate the collider
            }

            item.transform.position = point;
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
