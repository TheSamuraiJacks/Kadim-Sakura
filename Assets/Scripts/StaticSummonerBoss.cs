using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSummonerBoss : MonoBehaviour
{
    [Header("Boss Ayarları")]
    public float maxHealth = 100f;
    public Transform[] teleportPoints; // A, B, C, D Küpleri (Inspector'a sürükle)

    [Header("Yardakçı (Minion) Ayarları")]
    public GameObject minionPrefab; // Küçültülmüş EvilSamurai Prefab'ı
    public float spawnInterval = 5f; // Kaç saniyede bir adam çağırsın?
    public Transform spawnPoint; // Adamların çıkacağı nokta (Genelde boss'un önü)

    private float currentHealth;
    private int currentPhase = 0;
    private float nextSpawnTime;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;

        // Oyuncuyu bul (Sadece ona bakmak için)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Başlangıçta A noktasına (0. index) ışınlan
        if (teleportPoints.Length > 0)
        {
            transform.position = teleportPoints[0].position + Vector3.up * 1.5f; // Küpün biraz üstüne koy
        }
    }

    void Update()
    {
        // 1. OYUNCUYA BAK (Görsel efekt)
        if (player != null)
        {
            // Sadece Y ekseninde dönsün, havaya bakmasın
            Vector3 targetPostition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPostition);
        }

        // 2. YARDAKÇI ÇAĞIRMA (SPAWNER)
        if (Time.time >= nextSpawnTime)
        {
            SpawnMinion();
            nextSpawnTime = Time.time + spawnInterval;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20);
        }
    }

    void SpawnMinion()
    {
        if (minionPrefab == null) return;

        // Boss'un etrafında rastgele bir yerde doğsun
        Vector3 randomPos = Random.insideUnitSphere * 3f;
        randomPos.y = 0; // Yerde doğsunlar
        Vector3 finalSpawnPos = transform.position + randomPos;

        Instantiate(minionPrefab, finalSpawnPos, Quaternion.identity);
        Debug.Log("Boss yardımcısını çağırdı!");
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss Canı: " + currentHealth);

        // --- IŞINLANMA MANTIĞI ---

        if (currentHealth <= 75 && currentPhase < 1)
        {
            TeleportTo(1); // B Noktası
            currentPhase = 1;
        }
        else if (currentHealth <= 50 && currentPhase < 2)
        {
            TeleportTo(2); // C Noktası
            currentPhase = 2;
        }
        else if (currentHealth <= 25 && currentPhase < 3)
        {
            TeleportTo(3); // D Noktası
            currentPhase = 3;
            // Son fazda daha hızlı adam çağırsın mı? Çılgınlık başlasın!
            spawnInterval = 2f;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void TeleportTo(int index)
    {
        if (index < teleportPoints.Length)
        {
            // Direkt pozisyon değiştiriyoruz (NavMesh yok, özgürüz!)
            // Vector3.up * 1.5f ekliyoruz ki küpün içine gömülmesin, üstünde dursun
            transform.position = teleportPoints[index].position + Vector3.up * 1.5f;
            spawnPoint = transform;
            // Efekt (Partikül) varsa buraya Instantiate(smokeEffect...) ekleyebilirsin
        }
    }

    void Die()
    {
        // Ölüm animasyonu vs.
        Debug.Log("BOSS YENİLDİ!");
        Destroy(gameObject);
    }
}