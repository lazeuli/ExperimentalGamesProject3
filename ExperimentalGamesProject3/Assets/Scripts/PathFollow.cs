using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : MonoBehaviour
{
    public Transform[] path;
    public float speed;
    public float reachDist;
    public int currentPoint = 0;
    private Vector3 startingPoint;

    void Start()
    {
        startingPoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 directionVector = path[currentPoint].position - transform.position;
            directionVector.y = 0;
            if (directionVector.magnitude <= reachDist && currentPoint < path.Length - 1)
            {
                currentPoint++;
            }
            directionVector = directionVector.normalized;
            transform.position += directionVector * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 directionVector;
            if (currentPoint == 0)
            {
                directionVector = startingPoint - transform.position;
            }
            else
            {
                directionVector = path[currentPoint - 1].position - transform.position;
            }
            directionVector.y = 0;
            if (directionVector.magnitude <= reachDist && currentPoint > 0)
            {
                currentPoint--;
            }
            directionVector = directionVector.normalized;
            transform.position += directionVector * Time.deltaTime * speed;
        }
    }
}
