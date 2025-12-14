using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject evilSamuraiPrefab;
    public float spawnInterval = 5f;

    [Header("Optimization Settings")]
    public int totalEnemiesToSpawn = 8;    // O bölümde toplam kaç düşman çıkacak (Quota)
    public int maxConcurrentEnemies = 5;   // Aynı anda sahnede en fazla kaç düşman olabilir

    // Takip değişkenleri (Inspector'da görmen için Serialized yaptım ama private kalsınlar)
    [SerializeField] private int currentAliveCount = 0;   // Şu an yaşayan sayısı
    [SerializeField] private int totalSpawnedCount = 0;   // Toplam doğmuş olan sayısı

    [Header("Area Settings")]
    public float spawnRange = 10f;
    private Camera mainCamera;
    public Transform playerTransform;
    public int dayCount = 0;
    // Singleton mantığına gerek yok ama düşmanların spawner'a ulaşması için referans lazım
    // Bu yüzden static bir instance tutabiliriz VEYA düşman doğarken ona spawner'ı verebiliriz.
    // Şimdilik en temizi: Düşman doğarken ona "Ben senin sahibinim" demek.

    void Start()
    {
        mainCamera = Camera.main;

        // Spawn işlemini başlat
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        
    }
    void SpawnEnemy()
    {
        // 1. KONTROL: Eğer o bölüm için belirlenen toplam sayıya ulaştıysak spawn'ı durdur.
        if (totalSpawnedCount >= totalEnemiesToSpawn)
        {
            CancelInvoke(nameof(SpawnEnemy));
            DayManaging.instance.UploadScene();
            return;
        }

        // 2. KONTROL: Eğer sahnede zaten maksimum sayıda düşman varsa, yeni üretme, bekle.
        if (currentAliveCount >= maxConcurrentEnemies)
        {
            return;
        }

        FindPositionAndSpawn();
    }

    void FindPositionAndSpawn()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool isValidPosition = false;
        int attempts = 0;

        while (!isValidPosition && attempts < 10)
        {
            float randomX = Random.Range(-spawnRange, spawnRange);
            float randomZ = Random.Range(-spawnRange, spawnRange);
            if(playerTransform != null)
                spawnPosition = new Vector3(playerTransform.position.x + randomX, 0, playerTransform.position.z + randomZ);
            
            if (!IsVisibleToCamera(spawnPosition))
            {
                isValidPosition = true;
            }
            attempts++;
        }

        if (isValidPosition)
        {
            // Düşmanı oluşturuyoruz
            GameObject newEnemy = Instantiate(evilSamuraiPrefab, spawnPosition, Quaternion.identity);

            // --- KRİTİK NOKTA ---
            // Düşman scriptine ulaşıp, "Sen ölünce bana haber ver" dememiz lazım.
            // Senin düşman scriptinin adı "AttackController" idi, ona göre yazıyorum:
            AttackController enemyScript = newEnemy.GetComponent<AttackController>();
            if (enemyScript != null)
            {
                enemyScript.mySpawner = this; // Düşmana spawner referansını veriyoruz
            }

            // Sayaçları güncelle
            currentAliveCount++;
            totalSpawnedCount++;
        }
    }

    // Bu fonksiyonu Düşman (AttackController) scriptinden çağıracağız!
    public void OnEnemyKilled()
    {
        currentAliveCount--; // Yaşayan sayısını azalt ki yerine yenisi doğabilsin.

        // Güvenlik önlemi: Eksiye düşerse 0'a eşitle
        if (currentAliveCount < 0) currentAliveCount = 0;
    }

    bool IsVisibleToCamera(Vector3 position)
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(position);
        bool xInView = viewPos.x > 0 && viewPos.x < 1;
        bool yInView = viewPos.y > 0 && viewPos.y < 1;
        bool isInFront = viewPos.z > 0;
        return xInView && yInView && isInFront;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(spawnRange * 2, 1, spawnRange * 2));
    }
}