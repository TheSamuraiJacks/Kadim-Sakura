using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    float attackForce = 0;
    public Animator animator;
    public bool onUlt = false;

    bool isAlive = true;
    public float health = 100;

    public IAbility[] abilityList;

    // --- EKLEME 1: VFX'i bağlayacağımız kutuyu oluşturuyoruz ---
    [Header("Efekt Ayarları")]
    public ParticleSystem slashVFX;
    // -----------------------------------------------------------

    [HideInInspector] public Spawner mySpawner;

    private void Start()
    {
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

    void Attack()
    {
        attackForce++;
        if (attackForce > 2) attackForce = 0;

        animator.SetFloat("AttackForce", attackForce);

        if (onUlt)
            attackForce = 3;

        animator.SetTrigger("isAttacked");

        // --- EKLEME 2: Saldırı anında efekti çalıştırıyoruz ---
        if (slashVFX != null)
        {
            slashVFX.Play(); // Efekti oynat
        }
        // -----------------------------------------------------
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
            if (health <= 0) isAlive = false;
        }
        
        if (!isAlive) // Eğer öldüyse (daha önce ölmediyse)
        {
            animator.SetTrigger("isDead");

            // 2. BU KODU ÖLÜM KISMINA EKLE:
            // Spawner'a "Ben öldüm, sayımdan düş" diyoruz.
            if (mySpawner != null)
            {
                mySpawner.OnEnemyKilled();
                mySpawner = null; // Tekrar tekrar çağırmasın diye bağlantıyı kopar
            }

            // Destroy veya diğer işlemler...
            Destroy(gameObject, 5f); // Örnek
        }
    }
}