using Cinemachine;
using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject pivot;
    [SerializeField] private GameObject car;
    [SerializeField] private float carRotationSpeed;
    [SerializeField] private float pivotRotationSpeed;
    [SerializeField] private float carRotationSmoothness;
    [SerializeField] private float setSpeedForward;
    [SerializeField] private float driftMoveSpeed;
    [SerializeField] private float jumpMoveSpeed;
    [SerializeField] private float driftForce;
    [SerializeField] private GameObject[] skidMarks;
    [SerializeField] private float driftThreshold;
    [SerializeField] private float smoothFactorCarBody;
    [SerializeField] private float smooth;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rAmount;
    [SerializeField] private float collisionJumpForce;
    [SerializeField] private float hurdleJumpForce;
    [SerializeField] private GameObject[] carTyre;
    private Vector3 touchStartPositon;
    private float moveSpeedForward;
    private float touchDelta;
    private Vector3 moveDirection;
    private bool startMoving;
    private bool addForce = true;
    [SerializeField] private float raycastDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpTime;
    public bool isJump;
    public bool isGrounded;
    public float brakeSpeed = 0;
    public float initialSpeed = 0;
    private float currY;
    private float prevY;
    public bool isDead = false;
    public bool isCollidedCar;
    private void Start()
    {
        touchDelta = 0;
        StartCoroutine(SetInitialSpeed());
    }

    IEnumerator SetInitialSpeed()
    {

        while (initialSpeed >= 0)
        {
            initialSpeed -= Time.deltaTime * 10f;
            yield return null;
        }

        rb.useGravity = true;
    }

    void Update()
    {
        if (isDead) return;
        CheckGround();
        MoveForward();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            jumpTime += Time.deltaTime;
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPositon = new Vector3(touch.position.x, 0, 0);
                jumpTime = 0;
            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary && jumpTime >= 0.5f)
            {
                touchDelta = (touch.position.x - touchStartPositon.x) / Screen.width;
                RotateCar();
                if (touchDelta > 0)
                {
                    // RotateWheels(30f);
                }
                else if (touchDelta < 0)
                {
                    // RotateWheels(-30f);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (jumpTime < 0.5f)
                {
                    Jump();
                }
                touchDelta = 0;
                startMoving = true;
                smoothFactorCarBody = 0;
            }
        }

        ReAlignCarRotation();
    }
    private void Jump()
    {
        if (isGrounded)
        {
            //brakeSpeed = jumpMoveSpeed;
            isJump = true;
            Debug.Log("jumping");
            rb.AddForce(Vector3.up * hurdleJumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    private void ReAlignCarRotation()
    {
        if (startMoving)
        {
            smoothFactorCarBody += Time.deltaTime * smooth;
            Quaternion targetRotation = car.transform.rotation;
            pivot.transform.rotation = Quaternion.Slerp(pivot.transform.rotation, targetRotation, smoothFactorCarBody);
            // RotateWheels(0);
            if (Quaternion.Angle(pivot.transform.rotation, targetRotation) < 0.1f)
            {
                startMoving = false;
            }
        }
    }
    private void MoveForward()
    {
        if (Mathf.Abs(touchDelta) > driftThreshold)
        {
            if (addForce)
            {

                moveSpeedForward = driftMoveSpeed - brakeSpeed - initialSpeed;
                rb.AddForce(moveDirection * driftForce, ForceMode.VelocityChange);
                addForce = false;
            }
        }
        else
        {
            addForce = true;
            moveSpeedForward = setSpeedForward - brakeSpeed - initialSpeed;
        }
        if (!isCollidedCar)
        {
            moveDirection = pivot.transform.forward;
            transform.Translate(moveDirection * moveSpeedForward * Time.deltaTime);
        }
    }
    private void RotateCar()
    {
        pivot.transform.Rotate(Vector3.up, touchDelta * pivotRotationSpeed * Time.deltaTime);
        float desiredCarRotation = pivot.transform.rotation.eulerAngles.y + carRotationSpeed * touchDelta;
        Quaternion targetRotation = Quaternion.Euler(0, desiredCarRotation, 0);
        car.transform.rotation = Quaternion.Slerp(car.transform.rotation, targetRotation, carRotationSmoothness * Time.deltaTime);
    }
    private void CheckGround()
    {

        if (isJump)
        {
            currY = transform.position.y;
            if (currY < prevY)
            {
                Debug.Log("coming down");
                isJump = false;
                rb.AddForce(Vector3.down * 5f, ForceMode.VelocityChange);
            }
            else
            {
                Debug.Log("going up");
            }
            prevY = currY;
            return;
        }

        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            brakeSpeed = 0;
            isCollidedCar = false;
            isGrounded = true;
            transform.position = hit.point + Vector3.up * 0.1f;
            Debug.DrawRay(rayStart, Vector3.down * raycastDistance, Color.red);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) { 
        
        return;
        
        }
        if (other.gameObject.CompareTag("DeadZone"))
        {
            isDead = true;

            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;


            GameManager.Instance.RemovePlayer(gameObject);
            GameManager.Instance.GameEnd(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead || isCollidedCar) return;

        if (collision.collider.CompareTag("Car"))
        {
            isCollidedCar = true;
            Vector3 dir = (car.transform.position - collision.contacts[0].point);
            CarCollisionJump(dir.normalized + (Vector3.up));
            //carDetectionCollider.enabled = true;
        }
    }
    private void CarCollisionJump(Vector3 direction)
    {
        if (isGrounded)
        {
            isJump = true;
            Debug.Log("jumping from collision");

            rb.AddForce(direction * collisionJumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    //private void RotateWheels(float rAmount)
    //{
    //    for (int i = 0; i < carTyre.Length; i++)
    //    {
    //        carTyre[i].transform.localRotation = Quaternion.Euler(0, rAmount, 0);
    //    }
    //}
}

