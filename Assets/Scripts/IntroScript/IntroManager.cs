using UnityEngine;
using UnityEngine.UI; // Resimler için
using TMPro; // Yazılar için
using UnityEngine.SceneManagement; // Sahne değişimi için

public class IntroManager : MonoBehaviour
{
    [Header("Ekranda Değişecek Parçalar")]
    public Image mangaEkrani;        // Senin "Manga1" dediğin obje
    public TextMeshProUGUI altYazi;  // Altta hikayenin yazdığı yazı kutusu
    
    [Header("Sırasıyla İçerikler (Burayı Dolduracağız)")]
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
        // --- YENİ EKLENEN KISIM BAŞLANGIÇ ---
        // Eğer ses kaynağı hala aktifse (şarkı/konuşma bitmediyse)
        // Fonksiyondan "return" diyerek çık, yani aşağıdaki kodları çalıştırma.
        if (sesKaynagi.isPlaying)
        {
            return;
        }
        // --- YENİ EKLENEN KISIM BİTİŞ ---

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
        mangaEkrani.sprite = mangaResimleri[suankiSira];
        
        // 2. Resmi orjinal boyutuna/oranına getir (Yamulmasın diye)
        mangaEkrani.preserveAspect = true; 

        // 3. Yazıyı değiştir
        if (altYazi != null)
            altYazi.text = hikayeYazilari[suankiSira];

        // 4. Sesi çal (Eğer ses dosyası varsa)
        if (sesEfektleri.Length > suankiSira && sesEfektleri[suankiSira] != null)
        {
            sesKaynagi.Stop(); // Eski sesi sustur
            sesKaynagi.clip = sesEfektleri[suankiSira];
            sesKaynagi.Play(); // Yenisini çal
        }
    }
}