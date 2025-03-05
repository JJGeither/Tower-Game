using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerComponentManager : MonoBehaviour
{
    public static PlayerComponentManager Instance { get; private set; }
    public MovementController movementController;
    public InventoryController inventoryController;
    public Vector3 PlayerPosition { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (movementController == null) Debug.LogError("MovementController not assigned!");
        if (inventoryController == null) Debug.LogError("InventoryController not assigned!");
    }

    public void AddToInventory(GameObject item)
    {
        if (inventoryController != null)
        {
            inventoryController.AddToInventory(item);
        }
    }

    public void PlaceItem()
    {
        inventoryController.PlaceItem();
    }

    private void Update()
    {
        PlayerPosition = this.transform.position;
    }

}
