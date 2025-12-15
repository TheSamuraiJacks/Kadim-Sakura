using UnityEngine;
using UnityEngine.SceneManagement; // Sahne deðiþimi için gerekli kütüphane

public class LevelManager : MonoBehaviour
{
    public GameObject tuccarPanel; // Tüccar penceresini buraya sürükleyeceðiz


    // Bu fonksiyonu "Bölüm Bittiðinde" çaðýracaðýz
    public void BolumBitti()
    {
        // 1. Tüccar penceresini aç
        tuccarPanel.SetActive(true);

        // 2. Oyunu dondur (Arka planda karakter hareket etmesin)
        Time.timeScale = 0;
    }

    // Tüccarda iþimiz bitince "Sonraki Bölüm" butonuna basýnca bu çalýþacak
    public void SonrakiBolumeGec()
    {
        // Zamaný tekrar akýt (Yoksa yeni bölüm donuk baþlar)
        Time.timeScale = 1;

        // Mevcut sahnenin numarasýný al ve 1 ekle (Bir sonraki sahneyi yükle)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}