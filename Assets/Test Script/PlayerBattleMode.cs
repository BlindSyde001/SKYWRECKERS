using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleMode : MonoBehaviour
{
    public Collider[] attackHitBoxes;
    public int comboPoints = 0;
    private bool isRunning = false;
    private bool _weaponHitBox = false;
    public Animator anim;
    private float animTime;


    /// <summary>
    /// 1. Program the hitbox collider
    /// 2. Turn on and off hitbox
    /// 3. Start attack coroutine
    /// 4. Input attack sequences
    /// </summary>
    /// 


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            StartCoroutine("attack");
        }
    }

    #region 2. Hitbox Flicker Mechanism
    void weaponHitBox()
    {
        if (_weaponHitBox)
        {
            attackHitBoxes[comboPoints].gameObject.GetComponent<Collider>().enabled = true;
        }
        else if (!_weaponHitBox)
        {
            attackHitBoxes[comboPoints].gameObject.GetComponent<Collider>().enabled = false;
        }
    }
    #endregion

    #region 3. Attack Coroutine
    IEnumerator attack()
    {
        if (!isRunning)
        {
            isRunning = true;
            _weaponHitBox = true;
            animTrigger();
            yield return new WaitForSeconds(0.5f);
            _weaponHitBox = false;
            Debug.Log("COMBO = " + comboPoints);
            attackCombo();
            
            isRunning = false;
            //yield return null;
        }
    }
    #endregion

    #region 4. Attack Sequences
    void attackCombo()
    {
        if (comboPoints <= 1)
        {
            comboPoints++;
        }
        else
        {
            comboPoints = 0;
        }

    }
    #endregion

    void animTrigger()
    {
        if (comboPoints == 0 && isRunning)
        {
            anim.SetTrigger("Attack");
            Debug.Log("0");
            //anim.["Attack"].normalizedTime = 0.5f;
        }
        else if (comboPoints == 1 && isRunning)
        {
            anim.SetTrigger("Attack");
            Debug.Log("1");
        }
        else if (comboPoints == 2 && isRunning)
        {
            anim.SetTrigger("Attack");
            Debug.Log("2");
        }

    }

}
