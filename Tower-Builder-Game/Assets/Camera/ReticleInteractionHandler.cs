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
    public float sphereRadius = 0.5f;
    public Vector3 ReticleHitPoint { get; private set; }
    public Ray rayFromReticle { get; private set; }
    public Vector3 hitNormal;
    public RaycastHit hitInfo;

    // Minimum distance to the player
    public float minimumDistance = 2.0f;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        rayFromReticle = Camera.main.ScreenPointToRay(reticleUI.transform.position);

        if (Physics.SphereCast(rayFromReticle, sphereRadius, out hitInfo, raycastLength))
        {
            ReticleHitPoint = hitInfo.point;
            hitNormal = hitInfo.normal; // Get the normal of the hit point

            // Check if the hit point is closer than the minimum distance to the player (on the X and Z axes)
            float distanceToPlayer = Vector3.Distance(
                new Vector3(ReticleHitPoint.x, 0, ReticleHitPoint.z),
                new Vector3(PlayerComponentManager.Instance.transform.position.x, 0, PlayerComponentManager.Instance.transform.position.z)
            );

            if (distanceToPlayer < minimumDistance)
            {
                // Calculate the direction (ignoring the Y-axis)
                Vector3 direction = (
                    new Vector3(ReticleHitPoint.x, 0, ReticleHitPoint.z) -
                    new Vector3(PlayerComponentManager.Instance.transform.position.x, 0, PlayerComponentManager.Instance.transform.position.z)
                ).normalized;

                // Adjust the hit point to maintain the minimum distance along the X and Z axes
                ReticleHitPoint = new Vector3(
                    PlayerComponentManager.Instance.transform.position.x + direction.x * minimumDistance,
                    ReticleHitPoint.y, // Keep the Y position as is
                    PlayerComponentManager.Instance.transform.position.z + direction.z * minimumDistance
                );
            }

            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                interactionPrompt.enabled = true;
                InteractionController interactionController = hitInfo.collider.GetComponentInParent<InteractionController>();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactionController.Interact();
                }
            }
            else
            {
                interactionPrompt.enabled = false;
            }
        }
        else
        {
            interactionPrompt.enabled = false;
            ReticleHitPoint = rayFromReticle.GetPoint(raycastLength);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerComponentManager.Instance.PlaceItem();
        }
    }


    // Draws the ray in the Scene view for visualization
    private void OnDrawGizmos()
    {
        

        if (reticleUI != null)
        {
            Ray rayFromReticle = Camera.main.ScreenPointToRay(reticleUI.transform.position);
            Gizmos.color = Color.green;

            // Draw the ray
            Gizmos.DrawRay(rayFromReticle.origin, rayFromReticle.direction * raycastLength);

            RaycastHit hitInfo;
            if (Physics.SphereCast(rayFromReticle, sphereRadius, out hitInfo, raycastLength))
            {
                // Draw sphere at hit point
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(hitInfo.point, sphereRadius); // Smaller sphere to indicate the hit point
            }
        }
    }
}
