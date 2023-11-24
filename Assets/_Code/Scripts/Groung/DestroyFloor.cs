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

    [SerializeField] private Vector3 upPos;
    [SerializeField] private Vector3 downPos;

    private void Awake()
    {
        m_Material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        upPos = transform.position;
        downPos = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);
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

        Vector3 targetPosition = downPos;
        Vector3 secondTarget = upPos;
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
        yield return new WaitForSeconds(1.5f);
        while (Vector3.Distance(transform.position, targetPosition) > 0.1)
        {
            if (Vector3.Distance(transform.position, targetPosition) < 1)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }

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
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }

    public void ChangeColor()
    {
        m_Material.color = Color.green;
    }
}
