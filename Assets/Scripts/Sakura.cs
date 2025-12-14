using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura : MonoBehaviour
{
    float health = 500;
    bool isAlive = true;

    public void TakeDamage(float damage)
    {
        if (isAlive)
        {
            health -= damage;
            if(health <= 0) isAlive = false;
        }
        if (!isAlive)
        {
            //Gameover ekraný
        }

    }
}
