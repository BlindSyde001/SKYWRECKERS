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
    public float moveSpeed = 10f;

    private Vector3 startPosition;

    public List<Transform> movePoints;
    public int currentPos = 0;

    private Quaternion _lookRotation;
    private Vector3 _direction;
    private float rotateSpeed = 0.75f;

    //UPDATES

    private void Start()
    {

    }

    private void Update()
    {
        Patrol();
    }
        

    //METHODS

    void Patrol()
    {
        
        if (Vector3.Distance(movePoints[currentPos].position, transform.position) < 5)
        {
            currentPos++;
            if (currentPos >= movePoints.Count)
            {
                currentPos = 0;
            }
        }
        _direction = (movePoints[currentPos].position - transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, rotateSpeed * Time.deltaTime);

        Debug.Log(Vector3.Distance(transform.position, movePoints[currentPos].position));
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShipGround"))
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            direction.y = 0;
            other.GetComponent<MovementControlsShip>().nudgeVector = -direction * 150f;
        }
    }
}
