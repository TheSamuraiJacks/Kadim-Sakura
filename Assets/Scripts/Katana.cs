using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    public string TagName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == TagName)
        {
            if(other.gameObject.GetComponent<Enemy>() != null)
            other.gameObject.GetComponent<Enemy>().TakeDamage(10);

            if(other.gameObject.GetComponent<AttackController>() != null)
            other.gameObject.GetComponent<AttackController>().TakeDamage(10);
        }
    }
}
