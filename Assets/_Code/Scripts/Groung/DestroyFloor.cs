using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFloor : MonoBehaviour
{
    [SerializeField] private float lerpDuration;
    [SerializeField] private float lerpValue;
    private MeshRenderer floorMesh;
    [SerializeField] private Material RedMaterial;
     private Material whiteMaterial;
    [SerializeField] private Color startingColor;
    [SerializeField] private Color finalColor;
    private void Awake()
    {
        
        whiteMaterial = GetComponent<MeshRenderer>().material;
        floorMesh = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ChangePosition());
        }
    }
    //private void OnTriggerEnter(Collision collision)
    //{

    //}
    private IEnumerator ChangePosition()
    {
        
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);
        Vector3 secondTarget = transform.position;
        yield return new WaitForSeconds(0.1f);
        while (whiteMaterial.color != finalColor)
        {
            lerpDuration += Time.deltaTime;
            Color lerpedColor = Color.Lerp(startingColor, finalColor, lerpDuration);
            whiteMaterial.color = lerpedColor;
            yield return null;
        }
        lerpDuration = lerpValue;

        yield return new WaitForSeconds(0.2f);
        while (Vector3.Distance(transform.position, targetPosition) >= 0.1)
        {
            lerpDuration += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpDuration);
            yield return null;
        }
        transform.position = targetPosition;
        lerpDuration = lerpValue;

                                         // setting position back 
        whiteMaterial.color = Color.white;
        yield return new WaitForSeconds(3f);
        while (Vector3.Distance(secondTarget, transform.position) > 0.1)
        {
            lerpDuration += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, secondTarget, lerpDuration);
        }
        transform.position = secondTarget;
       // floorMesh.material = whiteMaterial;
        lerpDuration = lerpValue;


    }
}
