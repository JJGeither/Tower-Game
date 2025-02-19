using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float playerMaxSpeed;
    [SerializeField] private float playerMinSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float gravityScale;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundRayLength;

    private Rigidbody _rb;
    private bool isGrounded;
    private float _playerSpeed;
    private float _jumpForce;

    private void Start()
    {
        Application.targetFrameRate = 60;
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _jumpForce = jumpForce;
        }

        RotatePlayer(Input.GetAxis("Mouse X"));
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJumping();
        HandleGroundCheck();
    }

    private void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isMoving = verticalInput != 0 || horizontalInput != 0;

        float targetSpeed = isMoving ? _playerSpeed + acceleration * Time.fixedDeltaTime : _playerSpeed - deceleration * Time.fixedDeltaTime;
        _playerSpeed = Mathf.Clamp(targetSpeed, playerMinSpeed, playerMaxSpeed);

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * (_playerSpeed * Time.fixedDeltaTime);
        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        _rb.velocity = movement + Vector3.up * _jumpForce;
    }

    private void HandleJumping()
    {
        if (_jumpForce > 0)
        {
            if (!Input.GetKey(KeyCode.Space))
                _jumpForce /= 2;

            _jumpForce -= Time.fixedDeltaTime * 50;
            if (_jumpForce <= 0)
                _jumpForce = 0;
        }

        if (!isGrounded)
            _rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
    }

    private void HandleGroundCheck()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Vector3 rayOrigin = collider.bounds.center;
            float radius = collider.bounds.extents.x;
            isGrounded = Physics.SphereCast(rayOrigin, radius, Vector3.down, out _, groundRayLength);
        }
    }

    private void RotatePlayer(float mouseX)
    {
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Vector3 rayOrigin = collider.bounds.center;
            float radius = collider.bounds.extents.x;
            Gizmos.DrawWireSphere(rayOrigin + Vector3.down * groundRayLength, radius);
        }

        if (collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
