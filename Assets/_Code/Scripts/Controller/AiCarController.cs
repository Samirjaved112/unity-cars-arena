using System.Collections;
using System.Collections.Generic;
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

    private float moveSpeedForward;
    private Vector3 moveDirection;
    private bool addForce = true;
    public float brakeSpeed = 0;
    private Transform target; // Target point for the AI car to move towards
    private bool isBoundry;
    private void Start()
    {
        Application.targetFrameRate = 60;
        InitializeAI();
    }

    private void InitializeAI()
    {
        // For simplicity, set a default target point at the start
        target = new GameObject().transform;
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 rayStart = pivot.transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayStart, pivot.transform.forward, out hit, forwardRayDistance, boundaryLayer))
        {
            if (hit.collider.isTrigger)
            {
                if (hit.collider.CompareTag("Wall") && !isBoundry)
                {
                    isBoundry = true;
                    StartCoroutine(SteerTowardsTarget());

                }
                else
                {
                    // isBoundry = false;
                }
            }
        }
        MoveForward();

        ReAlignCarRotation();
        CheckGround();
    }

    private void MoveForward()
    {
        if (addForce)
        {
            moveSpeedForward = setSpeedForward - brakeSpeed;
            rb.AddForce(moveDirection * driftForce, ForceMode.VelocityChange);
            addForce = false;
        }

        moveDirection = pivot.transform.forward;
        transform.Translate(moveDirection * moveSpeedForward * Time.deltaTime);
    }

    private IEnumerator SteerTowardsTarget()
    {
        Vector3 startRot = pivot.transform.localEulerAngles;
        Vector3 startPos = new Vector3(pivot.transform.localPosition.x, 0, 0);
        Vector3 targetPos = new Vector3(pivot.transform.localPosition.x, 0, 0);
        targetPos.x -= 90;
        Vector3 directionToTarget = targetPos - startPos;
        Debug.Log("position start " + startPos);
        Debug.Log("position is " + targetPos);

        float startAngle = Mathf.DeltaAngle(startPos.x, startPos.x);
        float angleToTarget = Mathf.DeltaAngle(startPos.x, targetPos.x);
        Debug.Log("angle is " + startAngle);
        Debug.Log("angleToTarget is " + angleToTarget);
        float currentAngle = startAngle;
        Quaternion targetRotation = Quaternion.Euler(0, car.transform.rotation.eulerAngles.y + angleToTarget, 0);
        float lerpTime = 0f;
        while (lerpTime < 1)
        {
            lerpTime += carRotationSmoothness * Time.deltaTime;
            currentAngle = Mathf.Lerp(startAngle, angleToTarget, lerpTime);
            Debug.Log("currentAngle is " + currentAngle);
            pivot.transform.localRotation = Quaternion.Euler(0f, startRot.y + currentAngle, 0f);
            //pivot.transform.RotateAroundLocal(Vector3.up, currentAngle);
            //carRotationSmoothness += Time.deltaTime;
            car.transform.rotation = Quaternion.Lerp(car.transform.rotation, targetRotation, lerpTime);
            yield return null;
        }
        Debug.Log("local angle issss" + pivot.transform.eulerAngles.y);
        isBoundry = false;
    }

    private void ReAlignCarRotation()
    {
        // Implement any additional AI car realignment logic if needed
    }

    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            brakeSpeed = 0;
            transform.position = hit.point + Vector3.up * 0.1f;
            Debug.DrawRay(rayStart, Vector3.down * raycastDistance, Color.red);
        }
    }
}
