using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject evilSamuraiPrefab;
    public float spawnInterval = 5f;

    [Header("Optimization Settings")]
    public int totalEnemiesToSpawn = 15;
    public int maxConcurrentEnemies = 3;

    [Header("Debug View")]
    [SerializeField] private int currentAliveCount = 0;
    [SerializeField] private int totalSpawnedCount = 0;

    [Header("Area Settings")]
    public float spawnRange = 10f;
    private Camera mainCamera;
    public Transform playerTransform;

    void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);
<<<<<<< Updated upstream
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (totalSpawnedCount >= totalEnemiesToSpawn)
        {
            DayManaging.instance.UploadScene();
        }
        
=======

        // DontDestroyOnLoad genelde Managerlar içindir, Spawner her sahnede
        // ayrı olacağı için bunu kapatıyorum, yoksa sahneler arası karışıklık çıkarabilir.
        // DontDestroyOnLoad(this); 
>>>>>>> Stashed changes
    }

    void SpawnEnemy()
    {
        // 1. KONTROL: Toplam kota doldu mu?
        if (totalSpawnedCount >= totalEnemiesToSpawn)
        {
<<<<<<< Updated upstream
            CancelInvoke(nameof(SpawnEnemy));
=======
            // BURASI ÖNEMLİ: Kota doldu ama hemen leveli bitirme!
            // Sadece yaşayanların ölmesini bekle.
            // Eğer kimse kalmadıysa o zaman leveli bitir.
            if (currentAliveCount <= 0)
            {
                CancelInvoke(nameof(SpawnEnemy));
                Debug.Log("Tüm düşmanlar öldü, diğer güne geçiliyor...");

                // DayManager'da sahne geçişi yapacak fonksiyonun varsa çağır
                if (DayManaging.instance != null)
                    DayManaging.instance.UploadScene();
            }

            // Kota dolduysa yeni düşman üretme, fonksiyondan çık
>>>>>>> Stashed changes
            return;
        }

        // 2. KONTROL: Sahne kalabalık mı?
        if (currentAliveCount >= maxConcurrentEnemies)
        {
            return; // 3 kişi varsa basma, bekle. Biri ölünce basarsın.
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
            if (playerTransform != null)
                spawnPosition = new Vector3(playerTransform.position.x + randomX, 0, playerTransform.position.z + randomZ);

            if (!IsVisibleToCamera(spawnPosition))
            {
                isValidPosition = true;
            }
            attempts++;
        }

        if (isValidPosition)
        {
            GameObject newEnemy = Instantiate(evilSamuraiPrefab, spawnPosition, Quaternion.identity);

            // Düşmana "Ben senin sahibinim" diyoruz
            AttackController enemyScript = newEnemy.GetComponent<AttackController>();
            if (enemyScript != null)
            {
                enemyScript.mySpawner = this;
            }

            currentAliveCount++;
            totalSpawnedCount++;
        }
    }

    public void OnEnemyKilled()
    {
        currentAliveCount--;
        if (currentAliveCount < 0) currentAliveCount = 0;

        // Biri öldüğü an "Hadi hemen yenisini basalım" diye Invoke'u beklemeden tetikleyebilirsin (Opsiyonel)
        // SpawnEnemy(); 
    }

    bool IsVisibleToCamera(Vector3 position)
    {
        if (mainCamera == null) return false;
        Vector3 viewPos = mainCamera.WorldToViewportPoint(position);
        bool xInView = viewPos.x > 0 && viewPos.x < 1;
        bool yInView = viewPos.y > 0 && viewPos.y < 1;
        bool isInFront = viewPos.z > 0;
        return xInView && yInView && isInFront;
    }
}