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

    private Quaternion _lookRotation;
    private Vector3 _direction;
    public float speed;
    //UPDATES

    private void Start()
    {
        //transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        //transform.position += new Vector3(0, Mathf.Sin(Time.time * moveSpeed), 0);
        Rotation();
    }

    //METHODS

    void Rotation()
    {
        _direction = (testPoint.position - transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, speed * Time.deltaTime);
    }

}
