using UnityEngine;
using UnityEngine.UI; // Resimler için
using TMPro; // Yazılar için
using UnityEngine.SceneManagement;
using System.Collections; // Sahne değişimi ve Coroutine için

public class IntroManager : MonoBehaviour
{
    public float yazmaHizi = 0.1f; // Harfler ne kadar hızlı aksın?

    [Header("Ekranda Değişecek Parçalar")]
    public Image mangaEkrani;        // Senin "Manga1" dediğin obje
    public TextMeshProUGUI altYazi;  // Altta hikayenin yazdığı yazı kutusu

    [Header("Sırasıyla İçerikler")]
    public Sprite[] mangaResimleri;  // 4 resim dosyası buraya
    [TextArea(3, 10)]
    public string[] hikayeYazilari;  // 4 hikaye metni buraya
    public AudioClip[] sesEfektleri; // 4 ses dosyası buraya

    [Header("Ayarlar")]
    public string oyunSahnesiAdi = "Day1"; // Oyunun başladığı sahnenin adı

    private int suankiSira = 0;
    private AudioSource sesKaynagi;

    void Start()
    {
        // Ses çaları otomatik ekliyoruz
        sesKaynagi = gameObject.AddComponent<AudioSource>();

        // İlk sahneyi göstererek başla
        Guncelle();
    }

    // Butona basınca bu çalışacak
    public void Ileri()
    {
        // Eğer ses kaynağı hala aktifse (şarkı/konuşma bitmediyse) geçişi engelle
        if (sesKaynagi.isPlaying)
        {
            return;
        }

        suankiSira++; // Sırayı bir artır

        // Eğer resimler bittiyse oyuna geç
        if (suankiSira >= mangaResimleri.Length)
        {
            SceneManager.LoadScene(oyunSahnesiAdi);
        }
        else
        {
            // Bitmediyse yeni sayfayı göster
            Guncelle();
        }
    }

    void Guncelle()
    {
        // 1. Ekrandaki resmi değiştir
        if (suankiSira < mangaResimleri.Length)
        {
            mangaEkrani.sprite = mangaResimleri[suankiSira];
            mangaEkrani.preserveAspect = true;
        }

        // 2. Yazıyı DAKTİLO EFEKTİ ile değiştir (DÜZELTİLEN KISIM BURASI)
        if (altYazi != null && suankiSira < hikayeYazilari.Length)
        {
            // Eğer önceki yazı hala yazılıyorsa durdur (üst üste binmesin)
            StopAllCoroutines();
            // Yeni yazıyı harf harf yazmaya başla
            StartCoroutine(DaktiloEfekti(hikayeYazilari[suankiSira]));
        }

        // 3. Sesi çal (Eğer ses dosyası varsa)
        if (sesEfektleri.Length > suankiSira && sesEfektleri[suankiSira] != null)
        {
            sesKaynagi.Stop(); // Eski sesi sustur
            sesKaynagi.clip = sesEfektleri[suankiSira];
            sesKaynagi.Play(); // Yenisini çal
        }
        else if (sesEfektleri.Length == suankiSira)
        {
            // Burası senin özel kodun, aynen bıraktım
            if (DayManaging.instance != null)
                DayManaging.instance.UploadScene();
        }
    }

    // --- DAKTİLO MOTORU ---
    IEnumerator DaktiloEfekti(string gelecekMetin)
    {
        // A. Önce kutuyu temizle
        altYazi.text = "";

        // B. Harf harf yazmaya başla
        foreach (char harf in gelecekMetin)
        {
            altYazi.text += harf; // Bir harf ekle
            yield return new WaitForSeconds(yazmaHizi); // Bekle
        }
    }
}