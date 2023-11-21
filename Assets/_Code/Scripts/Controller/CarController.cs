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
    bool singleTap = false;
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
                moveSpeedForward = driftMoveSpeed;
                rb.AddForce(moveDirection * driftForce, ForceMode.VelocityChange);
                addForce = false;
            }
        }
        else
        {
            addForce = true;
            moveSpeedForward = setSpeedForward;
        }
        moveDirection = pivot.transform.forward;
        transform.Translate(moveDirection * moveSpeedForward * Time.deltaTime);
        if (Input.touchCount > 0)
        {
          
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && touch.tapCount == 1)
            {
                Debug.Log("now is true");
                singleTap = true;
            }
            if (touch.phase == TouchPhase.Began)
            {

                touchStartPositon = new Vector3(touch.position.x, 0, 0);
            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
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
                else if(touchDelta<0)
                {
                   // RotateWheels(-30f);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchDelta = 0;
                startMoving = true;
                smoothFactorCarBody = 0;
                if (singleTap)
                {
                    // Perform jump functionality here
                    raycastDistance = 0f;
                    Jump();
                     // Reset the singleTap flag
                }
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
    private void Jump()
    {
        Debug.Log("car jumping");
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        singleTap = false;
    }
    private void CheckGround()
    {
        if (singleTap)
        {
            return;
        }
        Debug.Log(raycastDistance);
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayer))
        {
          
            transform.position = hit.point + Vector3.up * 0.1f;
            Debug.DrawRay(rayStart, Vector3.down * raycastDistance, Color.green);
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

