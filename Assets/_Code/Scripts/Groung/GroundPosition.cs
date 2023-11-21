using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPosition : MonoBehaviour
{
    public float xOffset = 1f;
    public float zOffset;
    [SerializeField] private GameObject hexagonprefab;
    [SerializeField] private Transform spawnPoint;
    private Vector3 updatedPosition;
    [SerializeField] private int numOfRows;
    void Start()
    {
        AlignChildObjects();
    }

    void AlignChildObjects()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i); 
           
            if (i % 2 == 0)
            {
               
                updatedPosition = new Vector3(i * xOffset, child.position.y, child.position.z+zOffset);
                
            }
            else
            {
                updatedPosition = new Vector3(i * xOffset, child.position.y, child.position.z);
            }
            child.position = updatedPosition; 
            //GameObject hexagon = Instantiate(hexagonprefab, updatedPosition, spawnPoint.rotation);
            //hexagon.transform.SetParent(transform);
        }
    }
}
