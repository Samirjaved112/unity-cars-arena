using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowsAlignment : MonoBehaviour
{

    [SerializeField] private float zOffset;
    [SerializeField] private int numOfColumns;
    [SerializeField] private GameObject hexagonPrefab;
    [SerializeField] private Transform spawnPoint;
    void Start()
    {
        AlignChildObjects();
    }

    void AlignChildObjects()
    {
        for (int i = 0; i < numOfColumns; i++)
        {
            //Transform child = transform.GetChild(i);
            Vector3 newPosition = new Vector3(spawnPoint.position.x, spawnPoint.localPosition.y, i * zOffset);
            GameObject hexagon = Instantiate(hexagonPrefab, newPosition, spawnPoint.rotation);
            hexagon.transform.SetParent(transform);
            //child.localPosition = newPosition;
        }
    }
}
