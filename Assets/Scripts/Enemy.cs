using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Düşman Tipi Ayarları")]
    public float maxHealth = 100f;
    public float damage = 10f;
    public float attackSpeed = 1.5f;
    public float attackRange = 2.0f; // Ne kadar yakından vursun?

    [Header("Referanslar")]
    // GameManager yoksa burayı elle atamak zorunda kalma diye opsiyonel yaptım
    public int scoreValue = 10;

    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private CapsuleCollider myCollider;

    private float currentHealth;
    private float lastAttackTime;
    private bool isDead = false;

    public GameObject orbPrefab;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider>();

        // Oyuncuyu bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Başlangıç ayarlarını uygula
        currentHealth = maxHealth;
        agent.stoppingDistance = attackRange - 0.5f; // Saldırı menzilinden biraz önce dursun
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (!isDead)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // --- SALDIRI MENZİLİNDEYSEK ---
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true; // Dur
                anim.SetBool("IsMoving", false);

                // --- YENİ EKLENEN KISIM: YÜZÜNÜ OYUNCUYA DÖN ---
                // Y eksenini sıfırlıyoruz ki havaya/yere bakmasın
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    // Slerp ile yumuşakça dönmesini sağlıyoruz (5f dönüş hızıdır)
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
                // -----------------------------------------------

                // Saldırı zamanlaması
                if (Time.time > lastAttackTime + attackSpeed)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
            // --- UZAKTAYSAK (KOVALA) ---
            else
            {
                agent.isStopped = false; // Yürü
                anim.SetBool("IsMoving", true);
                agent.SetDestination(player.position);
            }
        }
       
    }

    void AttackPlayer()
    {
        anim.SetTrigger("Attack");
    }

    // Mermi veya kılıç buna değince çağıracak
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // İstersen buraya "Hit" animasyonu (Impact) ekleyebilirsin
         anim.SetTrigger("Hit"); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Puan ekleme (GameManager varsa)
        // if (GameManager.instance != null) GameManager.instance.AddScore(scoreValue);

        agent.isStopped = true;
        agent.enabled = false;
        //myCollider.enabled = false;

        anim.SetTrigger("Die");
        Instantiate(orbPrefab).transform.position = transform.position + Vector3.up * 0.5f;
        Destroy(gameObject, 3f);

    }
}