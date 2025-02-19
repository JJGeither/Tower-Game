using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public GameObject target;
    public float rotationSpeed = 2.0f;
    public Vector3 offset;
    public float minRotationY = -100f;
    public float maxRotationY = 60f;

    private Transform playerTransform;
    private float rotationY = 0.0f;
    private float rotationX = 0.0f;


    void Start()
    {
        playerTransform = target.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.position = playerTransform.position + offset;

        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY = Mathf.Clamp(rotationY - Input.GetAxis("Mouse Y") * rotationSpeed, minRotationY, maxRotationY);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
