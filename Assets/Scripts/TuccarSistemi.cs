using UnityEngine;
using TMPro;
using System; // TextMeshPro

public class TuccarSistemi : MonoBehaviour
{
    [Header("UI Ba�lant�lar�")]
    public TextMeshProUGUI paraText;       // Sa� �stteki toplam para yaz�s�
    public TextMeshProUGUI canFiyatiText;   // Sol kutudaki "Fiyat: 10" yaz�s�
    public TextMeshProUGUI hasarFiyatiText; // Sa� kutudaki "Fiyat: 25" yaz�s�
    public GameObject tuccarPanel;

    [Header("Ekonomi Ayarlar�")]
    public int baslangicParasi = 0;   // Oyun a��ld���nda ka� parayla ba�las�n?
    public int canFiyati = 10;       // �lk fiyat
    public int hasarFiyati = 25;     // �lk fiyat

    [Header("Zam Ayar� (0.2 = %20)")]
    [Range(0f, 1f)]
    public float zamOrani = 0.2f;     // %20 zam i�in 0.2, %50 i�in 0.5 yaz

    // Gizli de�i�kenler
    private float mevcutOrb;
    void Awake()
    {
        this.gameObject.SetActive(true);
        // Kay�t sistemini iptal ettik. Direkt ba�lang�� paras�yla ba�l�yoruz.
        mevcutOrb = PlayerPrefs.GetFloat("GainedOrbValue", 0);
        
        // Ekran� g�ncelle
        UIGuncelle();
    }

    // TEST ���N: M tu�u para verir
    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (Input.GetKeyDown(KeyCode.M))
        {
            ParaKazan(500);
        }
        // YEN� EKLENEN KISIM: 'P' tu�una bas�nca Paneli A�/Kapa
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Panel a��ksa kapat�r, kapal�ysa a�ar (Tersi i�lemi yapar)
            bool durum = tuccarPanel.activeSelf;
            tuccarPanel.SetActive(!durum);
        }
    }

    public void CanSatinAl()
    {
        if (mevcutOrb >= canFiyati)
        {
            ParaHarca(canFiyati);
            Debug.Log("Can Sat�n Al�nd�! Canlar fullendi.");
            float a = PlayerPrefs.GetFloat("maxHealth");
            a = a + 20;
            PlayerPrefs.SetFloat("maxHealth", a);
            // �rn: PlayerHealth.Heal();

            // ZAM YAPMA ZAMANI
            // Fiyat� %20 (veya ayarl� oran) art�r�p tam say�ya yuvarl�yoruz
            canFiyati = Mathf.RoundToInt(canFiyati * (1 + zamOrani));

            UIGuncelle(); // Yeni fiyat� ekrana yaz
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
            Debug.Log("G��lendirme Al�nd�!");
            // �rn: PlayerDamage.Increase();

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
        PlayerPrefs.SetFloat("GainedOrbValue", mevcutOrb);
        // Kay�t i�lemi (PlayerPrefs) S�L�ND�. Sadece anl�k d���yoruz.
    }

    public void ParaKazan(int miktar)
    {
        mevcutOrb += miktar;
        UIGuncelle();
    }

    public void AlisverisiBitir()
    {
        PlayerPrefs.SetFloat("GainedOrbValue", mevcutOrb);
        Time.timeScale = 1;
        DayManaging.instance.UploadScene();
    }

    // T�m yaz�lar� g�ncelleyen fonksiyon
    void UIGuncelle()
    {
        // Toplam paray� yaz
        paraText.text = mevcutOrb.ToString();

        // Yeni fiyatlar� etiketlere yaz
        // "\n" alt sat�ra ge�mek demektir.
        canFiyatiText.text = "CAN YENILE Fiyat: " + canFiyati.ToString();
        hasarFiyatiText.text = "G��LENDIRME Fiyat: " + hasarFiyati.ToString();
    }

    
}