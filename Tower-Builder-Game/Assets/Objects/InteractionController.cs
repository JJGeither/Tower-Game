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
}
