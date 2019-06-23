using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlaneMovement : MonoBehaviour
{

    public float speed = 45f;
    public float barrelRollTime = 1;
    public float tweenTime = 1;
    public Image speedLines;


    private void Start()
    {
        
    }

    private void Update()
    {
        Vector3 moveCam = transform.position - transform.forward * 10f + Vector3.up * 5f;
        float smoothing = 0.80f;
        Camera.main.transform.position = Camera.main.transform.position * smoothing + 
            moveCam * (1f - smoothing);
        Camera.main.transform.LookAt(transform.position + transform.forward * 30f);


        transform.position += transform.forward * Time.deltaTime * speed;
        speed -= transform.forward.y * Time.deltaTime * 40f;

        if(speed < 20f)
        {
            speed = 20f;
        }

        if(speed > 70f)
        {
            speed = 70f;
        }

        speedBoost();
        speedSlow();
        barrelRoll();

        transform.Rotate(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal") * 2);

        float terrainHeightPositional = Terrain.activeTerrain.SampleHeight(transform.position);

        if (terrainHeightPositional > transform.position.y)
            transform.position = new Vector3(transform.position.x, terrainHeightPositional, transform.position.z);
    }


    void speedBoost()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed++;
        }
            
    }

    void speedSlow()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            speed--;
        }
    }

    void barrelRoll()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            BarrelRoll(-1);
            speedEffect();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            BarrelRoll(1);
            speedEffect();
        }
    }

    void speedEffect()
    {
        speedLines.DOFade(0.6f, 0.4f);
        StartCoroutine(FinishBarrelRoll());
    }

    IEnumerator FinishBarrelRoll()
    {
        yield return new WaitForSeconds(barrelRollTime);
        speedLines.DOFade(0, tweenTime);
    }

    void BarrelRoll(int dir)
    {
        transform.DOLocalMoveX(transform.position.x + (dir * 50), 1);
        transform.DORotate(new Vector3(0, 0, 360 * - dir), barrelRollTime, RotateMode.LocalAxisAdd);
    }
}
