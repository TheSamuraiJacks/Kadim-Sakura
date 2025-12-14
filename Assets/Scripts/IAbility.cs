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
    public float teleportSpeed = 10;
    public GameObject prefab;
    GameObject currentGo;
    public override void Use()
    {
        if (!TeleportAvaible)
        {
            targetPos = go.transform.position;
            currentGo = Instantiate(prefab);
            currentGo.transform.position = new Vector3(targetPos.x, 0.1f, targetPos.z);
            TeleportAvaible = true;
        }
        else
        {
            go.GetComponent<CharacterController>().enabled = false;
            go.transform.position = targetPos;
            go.GetComponent<CharacterController>().enabled = true;
            Destroy(currentGo);
            TeleportAvaible = false;
        }
    }
    public override void Preparation(GameObject go)
    {
        this.go = go;
    }
}
[CreateAssetMenu(menuName = "CreateAbility/Dash")]
public class Dash : IAbility
{
    GameObject go;

    public override void Use()
    {
        go.GetComponent<Animator>().SetTrigger("isDashed");
        go.GetComponent<AttackController>().katana.ChangeDamage(20);
    }
    public override void Preparation(GameObject go)
    {
        this.go = go;
    }
}