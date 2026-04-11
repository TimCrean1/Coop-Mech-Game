using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyController : MonoBehaviour
{

    public GameObject[] legTargets;
    public GameObject[] legCubes;
    Vector3[] legPositions;
    Vector3[] legOriginalPositions;

    public float maxMoveDist = 2.5f;
    public int legMovementSmoothness = 4;

    List<int> nextIndexToMove = new List<int>();
    List<int> indexMoving = new List<int>();

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

            if (Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= maxMoveDist)
            {
                if (!nextIndexToMove.Contains(i) && !indexMoving.Contains(i))
                {
                    nextIndexToMove.Add(i);

                }
                else if (!indexMoving.Contains(i))
                {
                    legTargets[i].transform.position = legOriginalPositions[i];
                }
            }
        }
        if (nextIndexToMove.Count == 0 || indexMoving.Count != 0) {
            return;
        }

        Vector3 targetPosition = legCubes[nextIndexToMove[0]].transform.position;
        StartCoroutine(Step(nextIndexToMove[0], targetPosition));
    }

    IEnumerator Step(int index, Vector3 moveTo)
    {
        if (nextIndexToMove.Contains(index))
        {
            nextIndexToMove.Remove(index);
        }
        if (!indexMoving.Contains(index))
        {
            indexMoving.Add(index);
        }

        Vector3 startingPosition = legOriginalPositions[index];

        for(int i = 1; i <= legMovementSmoothness; i++)
        {
            legTargets[index].transform.position = Vector3.Lerp(startingPosition, moveTo, i / legMovementSmoothness);
            yield return new WaitForFixedUpdate();
        }

        legOriginalPositions[index] = moveTo;

        if (indexMoving.Contains(index)) {
            indexMoving.Remove(index);
        }
    }
}
