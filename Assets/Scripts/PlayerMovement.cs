using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// 1. 8D movement
    /// 2. Stop and face direction of movement
    /// </summary>
    #region Variables
    public float velocity = 5f;
    public float turnSpeed = 10;

    private Vector2 input;
    private float angle;
    private Quaternion targetRotation;
    private Transform cam;
    Rigidbody rb;
   
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        getInput();

        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1) return;
        {
            calculateDirection();
            rotate();
            Move();
        }
    }

    /// <summary>
    /// Input from movement keys
    /// </summary>
    void getInput()
    {
        {
            input.x = Input.GetAxisRaw("Horizontal Move");
            input.y = Input.GetAxisRaw("Vertical Move");
        }
    }

    /// <summary>
    /// direction relative to cmaera's rotation
    /// </summary>
    void calculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }
    /// <summary>
    /// Rotate towards calculated angle
    /// </summary>
    void rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    void Move()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

}
