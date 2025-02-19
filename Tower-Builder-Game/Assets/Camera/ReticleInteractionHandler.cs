using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReticleInteractionHandler : MonoBehaviour
{
    public static ReticleInteractionHandler Instance { get; private set; }
    public GameObject reticlePrefab;
    public TextMeshProUGUI interactionPrompt;
    public Image reticleUI;
    public int raycastLength = 8;
    float sphereRadius = 0.5f;
    public Vector3 ReticleHitPoint { get; private set; }
    public Ray rayFromReticle { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        rayFromReticle = Camera.main.ScreenPointToRay(reticleUI.transform.position);
        RaycastHit hitInfo;

        if (Physics.SphereCast(rayFromReticle, sphereRadius, out hitInfo, raycastLength))
        {
            ReticleHitPoint = hitInfo.point;

            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                interactionPrompt.enabled = true;
                InteractionController interactionController = hitInfo.collider.GetComponent<InteractionController>();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactionController.Interact();
                }
            }
            else
            {
                interactionPrompt.enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerComponentManager.Instance.PlaceItem(hitInfo.point);
            }
        }
        else
        {
            interactionPrompt.enabled = false;
            ReticleHitPoint = rayFromReticle.GetPoint(raycastLength);
        }
    }

    // Draws the ray in the Scene view for visualization
    private void OnDrawGizmos()
    {
        if (reticleUI != null)
        {
            Ray rayFromReticle = Camera.main.ScreenPointToRay(reticleUI.transform.position);
            Gizmos.color = Color.green; // Set the color of the ray
            Gizmos.DrawRay(rayFromReticle.origin, rayFromReticle.direction * raycastLength); // Draw a ray of 20 units long
        }
    }
}
