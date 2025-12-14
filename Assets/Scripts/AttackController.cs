using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    float attackForce = 0;
    public Animator animator;
    public bool onUlt = false;

    public bool isAlive = true;
    public float health = 100;

    public IAbility[] abilityList;

    [Header("Efekt Ayarları")]
    public ParticleSystem slashVFX;

    [Header("Ses Ayarları")]
    public AudioClip attackSound; // Saldırı sesi
    private AudioSource audioSource;

    [HideInInspector] public Spawner mySpawner;

    public Katana katana;

    private void Start()
    {
        // AudioSource bileşenini al veya ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        foreach (var ability in abilityList)
        {
            if (ability != null)
                ability.Preparation(gameObject);
        }

        // Oyun başladığında efekt yanlışlıkla çalışmasın diye durduruyoruz
        if (slashVFX != null) slashVFX.Stop();
    }

    private void Update()
    {
        if (isAlive)
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
                if (abilityList[1] != null)
                    abilityList[1].Use();
            }
        }
    }

    void Attack()
    {
        katana.ChangeDamage(10);
        attackForce++;
        if (attackForce > 2) attackForce = 0;

        animator.SetFloat("AttackForce", attackForce);

        if (onUlt)
            attackForce = 3;

        animator.SetTrigger("isAttacked");

        // Efekti oynat
        if (slashVFX != null)
        {
            slashVFX.Play();
        }

        // Sesi çal
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    void Block()
    {
        animator.SetTrigger("isBlocking 0");
        animator.SetBool("isBlocking", true);
    }

    public void TakeDamage(float dmg)
    {
        if (isAlive)
        {
            health -= dmg;
            animator.SetTrigger("isHurt");
            if (health <= 0) isAlive = false;

            if (!isAlive)
            {
                animator.SetTrigger("isDead");

                if (mySpawner != null)
                {
                    mySpawner.OnEnemyKilled();
                    mySpawner = null;
                }

                Destroy(gameObject, 5f);
            }
        }
        
    }
}