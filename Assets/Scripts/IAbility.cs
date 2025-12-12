using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAbility : ScriptableObject
{
    public abstract void Preparation(GameObject go);
    public abstract void Use();
}
[CreateAssetMenu(menuName = "CreateAbility/Teleport")]
public class Teleport : IAbility
{
    GameObject go;
    Vector3 targetPos;

    bool TeleportAvaible = false;
    public float teleportSpeed = 30;

    public override void Use()
    {
        if (!TeleportAvaible)
        {
            targetPos = go.transform.position;
            TeleportAvaible = true;
        }
        else
        {
            go.GetComponent<CharacterController>().enabled = false;
            go.transform.position = Vector3.Lerp(go.transform.position, targetPos, Time.deltaTime * teleportSpeed);
            go.GetComponent<CharacterController>().enabled = true;
            TeleportAvaible = false;
        }
    }
    public override void Preparation(GameObject go)
    {
        this.go = go;
    }
}