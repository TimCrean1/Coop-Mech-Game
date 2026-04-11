using System.Collections.Generic;
using UnityEngine;

public class bodyController : MonoBehaviour
{

    public GameObject[] legTargets;
    public GameObject[] letCubes;
    Vector3[] legPositions;
    Vector3[] legOriginalPositions;


    List<int> nextIndexToMove = new List<int>();
    List<int> currentlyMoving = new List<int>();

    private void Start()
    {
        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];

        for(int i = 0; i < legTargets.Length; i++)
        {
            legPositions[i] = legTargets[i].transform.position;
            legOriginalPositions[i] = legPositions[i];
        }
    }
    private void FixedUpdate()
    {
        moveLegs(); 
    }

    private void moveLegs()
    {
        for(int i = 0; i < legTargets.Length; i++)
        {
            legTargets[i].transform.position = legOriginalPositions[i];
        }
    }
}
