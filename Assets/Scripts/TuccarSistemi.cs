using UnityEngine;
using TMPro;
using System; // TextMeshPro

public class TuccarSistemi : MonoBehaviour
{
    [Header("UI Baðlantýlarý")]
    public TextMeshProUGUI paraText;       // Sað üstteki toplam para yazýsý
    public TextMeshProUGUI canFiyatiText;   // Sol kutudaki "Fiyat: 100" yazýsý
    public TextMeshProUGUI hasarFiyatiText; // Sað kutudaki "Fiyat: 250" yazýsý
    public GameObject tuccarPanel;

    [Header("Ekonomi Ayarlarý")]
    public int baslangicParasi = 0;   // Oyun açýldýðýnda kaç parayla baþlasýn?
    public int canFiyati = 100;       // Ýlk fiyat
    public int hasarFiyati = 250;     // Ýlk fiyat

    [Header("Zam Ayarý (0.2 = %20)")]
    [Range(0f, 1f)]
    public float zamOrani = 0.2f;     // %20 zam için 0.2, %50 için 0.5 yaz

    // Gizli deðiþkenler
    private int mevcutOrb;

    void Start()
    {
        // Kayýt sistemini iptal ettik. Direkt baþlangýç parasýyla baþlýyoruz.
        mevcutOrb = baslangicParasi;

        // Ekraný güncelle
        UIGuncelle();
    }

    // TEST ÝÇÝN: M tuþu para verir
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ParaKazan(500);
        }
        // YENÝ EKLENEN KISIM: 'P' tuþuna basýnca Paneli Aç/Kapa
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Panel açýksa kapatýr, kapalýysa açar (Tersi iþlemi yapar)
            bool durum = tuccarPanel.activeSelf;
            tuccarPanel.SetActive(!durum);
        }
    }

    public void CanSatinAl()
    {
        if (mevcutOrb >= canFiyati)
        {
            ParaHarca(canFiyati);
            Debug.Log("Can Satýn Alýndý! Canlar fullendi.");
            // Örn: PlayerHealth.Heal();

            // ZAM YAPMA ZAMANI
            // Fiyatý %20 (veya ayarlý oran) artýrýp tam sayýya yuvarlýyoruz
            canFiyati = Mathf.RoundToInt(canFiyati * (1 + zamOrani));

            UIGuncelle(); // Yeni fiyatý ekrana yaz
        }
        else
        {
            Debug.Log("Yetersiz Bakiye!");
        }
    }

    public void HasarSatinAl()
    {
        if (mevcutOrb >= hasarFiyati)
        {
            ParaHarca(hasarFiyati);
            Debug.Log("Güçlendirme Alýndý!");
            // Örn: PlayerDamage.Increase();

            // ZAM YAPMA ZAMANI
            hasarFiyati = Mathf.RoundToInt(hasarFiyati * (1 + zamOrani));

            UIGuncelle();
        }
        else
        {
            Debug.Log("Paran yetmiyor!");
        }
    }

    void ParaHarca(int miktar)
    {
        mevcutOrb -= miktar;
        // Kayýt iþlemi (PlayerPrefs) SÝLÝNDÝ. Sadece anlýk düþüyoruz.
    }

    public void ParaKazan(int miktar)
    {
        mevcutOrb += miktar;
        UIGuncelle();
    }

    public void AlisverisiBitir()
    {
        tuccarPanel.SetActive(false);
        Time.timeScale = 1;
    }

    // Tüm yazýlarý güncelleyen fonksiyon
    void UIGuncelle()
    {
        // Toplam parayý yaz
        paraText.text = mevcutOrb.ToString();

        // Yeni fiyatlarý etiketlere yaz
        // "\n" alt satýra geçmek demektir.
        canFiyatiText.text = "CAN YENÝLE\nFiyat: " + canFiyati.ToString();
        hasarFiyatiText.text = "GÜÇLENDÝRME\nFiyat: " + hasarFiyati.ToString();
    }

    
}