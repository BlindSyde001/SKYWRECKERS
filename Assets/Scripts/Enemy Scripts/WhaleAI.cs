using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WhaleAI : MonoBehaviour
{
    //moving up and down; moving forward; moving between points on the map


    //VARIABLES
    public Vector3 moveDirection;
    public float moveDistance;
    public float moveSpeed;

    private Vector3 startPosition;

    public List<Transform> movePoints;
    public Transform testPoint;

    //UPDATES

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        transform.position += Vector3.MoveTowards(transform.position, testPoint.position, 0.00001f * Time.deltaTime);
    }

    //METHODS
}
