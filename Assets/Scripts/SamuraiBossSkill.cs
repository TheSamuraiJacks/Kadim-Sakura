using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiBossSkill : MonoBehaviour
{
    [Header("Skill Ayarları")]
    public float skillDamage = 20f;
    public float skillRadius = 5f;
    public Transform impactPoint;
    public GameObject rippleVFX;

    // LayerMask satırını sildik, artık gerek yok.

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void CastSkill()
    {
        animator.SetTrigger("Skill");
    }

    public void TriggerImpact()
    {
        // Efekti oluştur
        if (rippleVFX != null && impactPoint != null)
        {
            Instantiate(rippleVFX, impactPoint.position, Quaternion.identity);
        }

        // --- DEĞİŞİKLİK BURADA ---
        // Artık "sadece Player layerı" demiyoruz, "önüne gelen her şeyi" (yer, duvar vs.) algıla diyoruz.
        Collider[] hitObjects = Physics.OverlapSphere(impactPoint.position, skillRadius);

        foreach (Collider hitObject in hitObjects)
        {
            // Filtrelemeyi burada yapıyoruz: "Çarptığım şeyin etiketi Player mı?"
            if (hitObject.CompareTag("Player"))
            {
                // Evet Player'mış, o zaman canını yakalım
                AttackController playerHealth = hitObject.GetComponent<AttackController>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(skillDamage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (impactPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(impactPoint.position, skillRadius);
        }
    }
}