using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveNPCEnd : MonoBehaviour
{
    public Transform targetPosition;
    public float moveDuration = 8f;

    private Vector3 startPosition;
    private float timeSinceStart;

    private void Start()
    {
        startPosition = transform.position;
        timeSinceStart = 0f;
    }

    private void Update()
    {
        if (timeSinceStart <= moveDuration)
        {
            timeSinceStart += Time.deltaTime;
            float t = timeSinceStart / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition.position, t);
        }
    }
}