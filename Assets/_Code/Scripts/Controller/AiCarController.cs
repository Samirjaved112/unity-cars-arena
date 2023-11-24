using System.Collections;
using UnityEngine;

public class AiCarController : MonoBehaviour
{
    [SerializeField] private GameObject pivot;
    [SerializeField] private GameObject car;
    [SerializeField] private float carRotationSpeed;
    [SerializeField] private float pivotRotationSpeed;
    [SerializeField] private float carRotationSmoothness;
    [SerializeField] private float setSpeedForward;
    [SerializeField] private float driftForce;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float raycastDistance;
    [SerializeField] private float forwardRayDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask boundaryLayer;
    [SerializeField] private BoxCollider carDetectionCollider;
    [SerializeField] private float collisionJumpForce;
    [SerializeField] private float hurdleJumpForce;

    private float moveSpeedForward;
    private Vector3 moveDirection;
    private bool addForce = true;
    public float brakeSpeed = 0;

    private bool isBoundry;
    public bool isGrounded;
    public bool isCollidedCar;
    private bool isJump;
    public float initialSpeed = 0;

    public bool isDead = false;
    public float prevY,currY;

    private void Start()
    {
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

    private void Update()
    {
        if (isDead) return;
        MoveForward();
        Jump();
        CheckBoundary();

        CheckGround();
    }

    private void MoveForward()
    {
        moveSpeedForward = setSpeedForward - brakeSpeed - initialSpeed;
        //if (addForce)
        //{
        //    rb.AddForce(moveDirection * driftForce, ForceMode.VelocityChange);
        //    addForce = false;
        //}

        if (!isCollidedCar)
        {
            moveDirection = pivot.transform.forward;
            transform.Translate(moveDirection * moveSpeedForward * Time.deltaTime);
        }
    }

    private void Jump()
    {

        RaycastHit hit;
        Vector3 rayStart = car.transform.position + Vector3.up * 1.5f + car.transform.forward * 4f;
        Vector3 direction = (-car.transform.up).normalized;
        if (Physics.Raycast(rayStart, direction, out hit, 20f, groundLayer))
        {
            if (hit.collider.CompareTag("RedHex"))
            {
                if (isGrounded)
                {
                    isJump = true;
                    Debug.Log("jumping from red");
                    rb.AddForce(Vector3.up * hurdleJumpForce, ForceMode.Impulse);
                    isGrounded = false;
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                isJump = true;
                Debug.Log("jumping from empty");

                rb.AddForce(Vector3.up * 12f, ForceMode.Impulse);
                isGrounded = false;
            }
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
    private IEnumerator SteerTowardsTarget(float xCount)
    {
        Vector3 startRot = pivot.transform.localEulerAngles;
        Vector3 startPos = new Vector3(pivot.transform.localPosition.x, 0, 0);
        Vector3 targetPos = new Vector3(pivot.transform.localPosition.x, 0, 0);
        targetPos.x += xCount;
        Vector3 directionToTarget = targetPos - startPos;


        float startAngle = Mathf.DeltaAngle(startPos.x, startPos.x);
        float angleToTarget = Mathf.DeltaAngle(startPos.x, targetPos.x);

        float currentAngle = startAngle;
        Quaternion targetRotation = Quaternion.Euler(0, car.transform.rotation.eulerAngles.y + angleToTarget, 0);
        float lerpTime = 0f;
        while (lerpTime < 1)
        {
            lerpTime += carRotationSmoothness * Time.deltaTime;
            currentAngle = Mathf.Lerp(startAngle, angleToTarget, lerpTime);

            pivot.transform.localRotation = Quaternion.Euler(0f, startRot.y + currentAngle, 0f);
            car.transform.rotation = Quaternion.Lerp(car.transform.rotation, targetRotation, lerpTime);
            yield return null;
        }

        isBoundry = false;
    }

    private void CheckBoundary()
    {
        RaycastHit hit;
        Vector3 rayStart = pivot.transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, pivot.transform.forward, out hit, forwardRayDistance, boundaryLayer))
        {
            if (hit.collider.CompareTag("Wall") && !isBoundry)
            {
                isBoundry = true;
                float dir = GetBestDirection();
                float angle = Random.Range(90f, 180f);

                pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, car.transform.rotation, 1f);
                StartCoroutine(SteerTowardsTarget(angle * dir));
            }
        }
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
            isGrounded = true;
            isCollidedCar = false;
            brakeSpeed = 0;
            transform.position = hit.point + Vector3.up * 0.1f;
        }
    }

    private float GetBestDirection()
    {
        float rightDist = 0f;
        float leftDist = 0f;
        RaycastHit hit;
        Vector3 rayStart = pivot.transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, pivot.transform.right, out hit, 150f, boundaryLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                rightDist = hit.distance;
            }
        }
        rayStart = pivot.transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, -pivot.transform.right, out hit, 150f, boundaryLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                leftDist = hit.distance;
            }
        }

        return (leftDist >= rightDist) ? -1f : 1f;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.gameObject.CompareTag("DeadZone"))
        {
            isDead = true;
            GameManager.Instance.RemovePlayer(this.gameObject);
        }
        if (other.gameObject.CompareTag("Car"))
        {
            Vector3 rightVector = gameObject.transform.right;
            Vector3 targetPoint = gameObject.transform.position - other.gameObject.transform.position;
            // Align Pivot & Car
            pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, car.transform.rotation, 1f);
            // Calculate the dot product
            float dotProduct = Vector3.Dot(targetPoint, rightVector);
            if (dotProduct > 0)
            {
                StartCoroutine(SteerTowardsTarget(-90));
            }
            else if (dotProduct < 0)
            {
                StartCoroutine(SteerTowardsTarget(90));
            }
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
}
