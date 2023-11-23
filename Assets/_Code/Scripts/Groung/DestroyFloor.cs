using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFloor : MonoBehaviour
{
    [SerializeField] private Material m_Material;

    [SerializeField] private float lerpDuration;
    [SerializeField] private float lerpValue;

    [SerializeField] private Color whiteColor;
    [SerializeField] private Color redColor;


    private void Awake()
    {
        m_Material = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CarTrig"))
        {
            StartCoroutine(ChangePosition());
        }
    }

    private IEnumerator ChangePosition()
    {

        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);
        Vector3 secondTarget = transform.position;
        yield return new WaitForSeconds(0.1f);


        while (m_Material.color != redColor)
        {
            lerpDuration += Time.deltaTime;
            Color lerpedColor = Color.Lerp(whiteColor, redColor, lerpDuration);
            m_Material.color = lerpedColor;
            yield return null;
        }

        // Change Platform Tag to RedHex
        tag = "RedHex";

        lerpDuration = lerpValue;

        // Moving Downward
        yield return new WaitForSeconds(0.2f);
        while (Vector3.Distance(transform.position, targetPosition) >= 0.1)
        {
            lerpDuration += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpDuration);
            yield return null;
        }
        transform.position = targetPosition;
        lerpDuration = lerpValue;

        m_Material.color = Color.white;

        // Moving Upward
        yield return new WaitForSeconds(3f);
        while (Vector3.Distance(secondTarget, transform.position) > 0.1)
        {
            lerpDuration += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, secondTarget, lerpDuration);
        }
        transform.position = secondTarget;
        lerpDuration = lerpValue;

        // Change Platform Tag to WhiteHex
        tag = "WhiteHex";

    }

    public void ChangeColor()
    {
        m_Material.color = Color.green;
    }
}
