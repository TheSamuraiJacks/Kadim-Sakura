using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    float attackForce = 0;
    public Animator animator;
    public bool onUlt = false;

    bool isAlive = true;
    float health = 100;

    public IAbility[] abilityList;

    private void Start()
    {
        foreach (var ability in abilityList)
        {
            if (ability != null)
            ability.Preparation(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Attack();
        

        if (Input.GetMouseButtonDown(1))
            Block();
        

        if (Input.GetMouseButtonUp(1))
            animator.SetBool("isBlocking", false);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (abilityList[0] != null)
                abilityList[0].Use();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(abilityList[1] != null)
            abilityList[1].Use();
        }
    }

    void Attack()
    {
        attackForce++;
        if (attackForce > 2) attackForce = 0;

        animator.SetFloat("AttackForce", attackForce);

        if (onUlt)
            attackForce = 3;

        animator.SetTrigger("isAttacked");
    }

    void Block()
    {
        animator.SetTrigger("isBlocking 0");
        animator.SetBool("isBlocking", true);
    }

}
