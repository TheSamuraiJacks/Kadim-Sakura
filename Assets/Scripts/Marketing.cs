using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marketing : MonoBehaviour
{
    float currentOrbs;
    void Start()
    {
        currentOrbs = PlayerPrefs.GetFloat("GainedOrbValue", 0);
        currentOrbs = Mathf.Max(0, currentOrbs);
    }

}
