using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public float gainValue;
    public Material evilMaterial;
    private void OnEnable()
    {
        gainValue = Random.Range(5,20);
        Invoke(nameof(TurnEvil), 5);
        Destroy(gameObject,15);
    }

    void TurnEvil()
    {
        gainValue = -gainValue;
        gameObject.GetComponent<MeshRenderer>().material = evilMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;

        if (go.tag == "Player")
        {
            go.GetComponent<AttackController>().health += gainValue;
            Destroy(gameObject);
        }
    }
}
