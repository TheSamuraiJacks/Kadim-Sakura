using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
