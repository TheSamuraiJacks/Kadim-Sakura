using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // BU SATIR EKSİKTİ (NavMesh için şart)

public class SamuraiBossAI : MonoBehaviour
{
    [Header("Gerekli Bileşenler")]
    public Transform player;// Kovalayacağı oyuncu

    // --- DÜZELTME BURADA ---
    // Eski hali: public BossSkill bossSkillScript;
    // Yeni hali:
    public SamuraiBossSkill bossSkillScript;

    [Header("Mesafe Ayarları")]
    public float chaseRange = 15f;    // Oyuncuyu ne kadar uzaktan fark etsin?
    public float attackRange = 2.5f;  // Normal vuruş için ne kadar yaklaşmalı?
    public float skillRange = 6f;     // Skill atmak için ne kadar yaklaşmalı?

    [Header("Zamanlama (Cooldown)")]
    public float skillCooldown = 10f; // Kaç saniyede bir skill atabilir?
    public float attackCooldown = 2f; // Normal vuruş hızı

    // Gizli değişkenler
    private NavMeshAgent agent;
    private Animator animator;
    private float nextSkillTime = 0f;
    private float nextAttackTime = 0f;
    private float distanceToPlayer;

    public float currentHealth = 100;
    public bool isAlive = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Eğer player atanmadıysa otomatik bulmayı dene (Tag ile)
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. Oyuncuyla aradaki mesafeyi ölç
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 2. Animasyon için hız bilgisini gönder (Idle/Run geçişi için)
        // NavMesh hızı 0.1'den büyükse koşuyordur
        animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);

        // KARAR MEKANİZMASI
        if (distanceToPlayer <= skillRange && distanceToPlayer > 3f && Time.time >= nextSkillTime)
        {
            PerformSkill();
        }
        else if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            PerformAttack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Hiçbiri değilse ve menzildeyse: KOVALA
            ChasePlayer();
        }
        else
        {
            // Oyuncu çok uzaktaysa: DUR
            agent.isStopped = true;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isAlive = false;
            animator.SetTrigger("Die");
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false; // Hareketi aç
        agent.SetDestination(player.position); // Oyuncuya git
    }

    void PerformAttack()
    {
        // Vururken durmalı
        agent.isStopped = true;

        // Oyuncuya dön (Yüzünü dönmezse boşa vurur)
        FaceTarget();

        // Normal saldırı animasyonunu tetiklee
        animator.SetTrigger("Attack"); // Senin animatordaki parametren "Attack" idi

        // Bir sonraki vuruş için zamanı ileri at
        nextAttackTime = Time.time + attackCooldown;
    }

    void PerformSkill()
    {
        FaceTarget(); // Son kez dön

        // Skill kodunu çağır
        if (bossSkillScript != null)
        {
            bossSkillScript.CastSkill();
        }

        nextSkillTime = Time.time + skillCooldown;
    }

    // Animation Event ile çağrılan fonksiyon
    public void FinishSkill()
    {


        // 5. Küçük bir temizlik (Hızını sıfırla ki kaymasın)
        if (agent.isOnNavMesh)
        {
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }    // Boss'un aniden oyuncuya dönmesini sağlayan fonksiyon
    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        // Direction sıfırsa hata vermemesi için kontrol ekleyelim
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // Editörde alanları görmek için çizgiler
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Saldırı menzili

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, skillRange);  // Skill menzili
    }
}