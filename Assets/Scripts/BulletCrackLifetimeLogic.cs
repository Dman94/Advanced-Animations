using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCrackLifetimeLogic : MonoBehaviour
{
    const float MAX_LIFETIME = 10.00f;
    float Current_Lifetime = MAX_LIFETIME;

     void Update()
    {
        Current_Lifetime -= Time.deltaTime;

        if(Current_Lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
