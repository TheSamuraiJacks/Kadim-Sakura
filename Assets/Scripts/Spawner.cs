using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject evilSamuraiPrefab; // Buraya EvilSamurai prefabını sürükle
    public float spawnInterval = 5f;     // Kaç saniyede bir çıkacağı

    [Header("Area Settings")]
    public float spawnRange = 10f;       // Alan genişliği
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Sahnedeki ana kamerayı bul

        // Fonksiyon ismini string yerine nameof() ile çağırmak hata yapmanı engeller
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);
    }

    void SpawnEnemy()
    {
        FindPositionAndSpawn();
    }

    void FindPositionAndSpawn()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool isValidPosition = false;
        int attempts = 0;

        // Kameranın görmediği bir yer bulana kadar 10 kere dener
        while (!isValidPosition && attempts < 10)
        {
            float randomX = Random.Range(-spawnRange, spawnRange);
            float randomZ = Random.Range(-spawnRange, spawnRange);
            spawnPosition = new Vector3(randomX, 0, randomZ);

            // Eğer bu nokta ekranda GÖRÜNMÜYORSA, yer uygundur
            if (!IsVisibleToCamera(spawnPosition))
            {
                isValidPosition = true;
            }
            attempts++;
        }

        // Eğer uygun yer bulduysak (veya 10 deneme bittiyse) üret
        if (isValidPosition)
        {
            Instantiate(evilSamuraiPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Bir noktanın kamerada görünüp görünmediğini kontrol eden fonksiyon
    bool IsVisibleToCamera(Vector3 position)
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(position);

        // Viewport koordinatlarında (0,0) sol alt, (1,1) sağ üsttür.
        // Eğer nokta 0 ile 1 arasındaysa ekrandadır.
        bool xInView = viewPos.x > 0 && viewPos.x < 1;
        bool yInView = viewPos.y > 0 && viewPos.y < 1;
        bool isInFront = viewPos.z > 0; // Kameranın önünde mi?

        return xInView && yInView && isInFront;
    }

    // Editörde spawn alanını görebilmen için yardımcı çizim (Gizmos)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Alanı temsil eden tel kafes küp çizer
        Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(spawnRange * 2, 1, spawnRange * 2));
    }
}