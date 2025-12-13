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
            other.gameObject.GetComponent<Enemy>().TakeDamage(10);
        }
    }
}
