using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour
{
    //VARIABLES
    public GameObject smokeFX;
    public GameObject emberFX;
    public ParticleSystem smoke;
    public ParticleSystem ember;

    //UPDATES
    private void Start()
    {
        StartCoroutine(ExplosionInstance());
    }

    //METHODS
    private IEnumerator ExplosionInstance()
    {
        smoke.enableEmission = true;
        if (ember != null)
            ember.enableEmission = true;
        yield return new WaitForSeconds(0.5f);
        smoke.enableEmission = false;
        if (ember != null)
            ember.enableEmission = false;
        Destroy(gameObject, 0.5f);
        yield return null;
    }
}
