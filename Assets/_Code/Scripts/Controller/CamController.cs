using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float lerpValue;
    public float rotationSpeed;
    private void LateUpdate()
    {

       // Vector3 desPos = target.position + offset;
        //Quaternion desRot = target.rotation;
        //transform.position = Vector3.Lerp(transform.position, desPos, lerpValue);
        //transform.LookAt(target);
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);

        // Rotate the camera to match the player's rotation
        Quaternion targetRotation = Quaternion.LookRotation(target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, desRot, lerpValue);
        // transform.rotation = Vector3.Lerp(transform.)
    }
}
