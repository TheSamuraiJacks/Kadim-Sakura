using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Slider için gerekli

public class VolumeController : MonoBehaviour
{
    [Header("UI Slider Bağlantıları")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 1. Oyun açılınca slider çubuklarını kayıtlı seviyeye getir
        // (Yoksa ses %50 iken slider %100 görünebilir, bu kötü bir deneyimdir)

        if (musicSlider != null)
        {
            // AudioManager'da kullandığımız kayıt isminin aynısı olmalı: "MusicVolumeSave"
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolumeSave", 0.75f);

            // Slider oynatılınca çalışacak fonksiyonu kodla bağlıyoruz (Daha sağlam)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolumeSave", 0.75f);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // Slider bu fonksiyonu çağıracak
    public void SetMusicVolume(float value)
    {
        // AudioManager'daki yeni fonksiyonu çağırıyoruz
        AudioManager.instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
    }
}