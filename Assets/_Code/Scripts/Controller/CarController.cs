using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
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
    public float brakeSpeed= 0;

    private void Start()
    {
        touchDelta = 0;
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        if (Mathf.Abs(touchDelta) > driftThreshold)
        {
            if (addForce)
            {
                moveSpeedForward = driftMoveSpeed- brakeSpeed;
                rb.AddForce(moveDirection * driftForce, ForceMode.VelocityChange);
                addForce = false;
            }
        }
        else
        {
            addForce = true;
            moveSpeedForward = setSpeedForward-brakeSpeed;
        }
        moveDirection = pivot.transform.forward;
        transform.Translate(moveDirection * moveSpeedForward * Time.deltaTime);

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
                pivot.transform.Rotate(Vector3.up, touchDelta * pivotRotationSpeed * Time.deltaTime);
                float desiredCarRotation = pivot.transform.rotation.eulerAngles.y + carRotationSpeed * touchDelta;
                Quaternion targetRotation = Quaternion.Euler(0, desiredCarRotation, 0);
                car.transform.rotation = Quaternion.Slerp(car.transform.rotation, targetRotation, carRotationSmoothness * Time.deltaTime);
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
                    if (isGrounded)
                    {
                        brakeSpeed = jumpMoveSpeed;
                        isJump = true;
                        Debug.Log("jumping");
                        rb.AddForce(Vector3.up * 20f, ForceMode.Impulse);
                        Physics.gravity = Vector3.down * 15f; 
                        isGrounded = false;
                    }
                }
                touchDelta = 0;
                startMoving = true;
                smoothFactorCarBody = 0;

            }
        }
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
        CheckGround();
    }

    private void CheckGround()
    {
        if (isJump)
        {
            RaycastHit hit2;
            Vector3 rayStart2 = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(rayStart2, Vector3.down, out hit2, raycastDistance*10f, groundLayer))
            {
                Debug.Log("is hitting");
                if (Vector3.Distance(transform.position, hit2.point) > 2f)
                {
                isJump = false;
                }
                else
                {
                    return;

                }
            }
            else
            {
                return;

            }

        }
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            brakeSpeed = 0;
            Physics.gravity = Vector3.down * 9.81f;

            isGrounded = true;
            transform.position = hit.point + Vector3.up * 0.1f;
            Debug.DrawRay(rayStart, Vector3.down * raycastDistance, Color.red);
        }
        else
        {
            // setSpeedForward =0;
            //transform.position = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
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

