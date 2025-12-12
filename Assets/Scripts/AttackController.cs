using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    float attackForce = 0;
    public Animator animator;
    public bool onUlt = false;

    [Header("Teleport")]
    public Vector3 targetPos = Vector3.zero;
    bool TeleportAvaible = false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Block();
        }

        if (Input.GetMouseButtonUp(1))
            animator.SetBool("isBlocking", false);

        

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!TeleportAvaible)
            {
                SetTargetPos();
            }
            else
            {
                Teleport();
            }
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

    void SetTargetPos()
    {
        targetPos = transform.position;
        TeleportAvaible = true;
    }

    void Teleport()
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = targetPos;
        gameObject.GetComponent<CharacterController>().enabled = true;
        TeleportAvaible = false;
    }

}
