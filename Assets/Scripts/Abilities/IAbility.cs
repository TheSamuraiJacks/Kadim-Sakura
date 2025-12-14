using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAbility : ScriptableObject
{
    public abstract void Preparation(GameObject go);
    public abstract void Use();
}

