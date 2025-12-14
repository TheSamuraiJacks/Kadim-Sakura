using System.Collections; // Zamanlama iþlemleri için þart
using UnityEngine;
using TMPro; // TextMeshPro kullanýyoruz

public class DaktiloYazi : MonoBehaviour
{
    [Header("Ayarlar")]
    public TextMeshProUGUI yaziKutusu; // Ekrandaki Text objen
    public float yazmaHizi = 0.05f; // Harfler ne kadar hýzlý gelsin? (Düþük sayý = Hýzlý)

    [TextArea] // Inspector'da geniþ kutu açar, rahat yazarsýn
    public string yazilacakMetin; // Manga panelinde ne yazacak?

    void OnEnable() // Bu obje her açýldýðýnda (Her yeni panelde) çalýþýr
    {
        // Önce yazý yazma iþlemini baþlat
        StartCoroutine(YazmayaBasla());
    }

    IEnumerator YazmayaBasla()
    {
        // 1. Kutuyu temizle (Eski yazý kalmasýn)
        yaziKutusu.text = "";

        // 2. Harf harf döngüye gir
        // "yazilacakMetin" içindeki her bir harfi al...
        foreach (char harf in yazilacakMetin)
        {
            yaziKutusu.text += harf; // Kutudaki yazýya o harfi ekle
            yield return new WaitForSeconds(yazmaHizi); // Bekle (0.05 saniye)
        }
    }
}