using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Ayarlar")]
    public AudioMixer mainMixer; // Oluşturduğun Mikseri buraya sürükle
    public Sound[] sounds;       // Ses listesi (Önceki mesajdaki Sound sınıfı)

    private const string MIXER_MUSIC = "MusicVol";
    private const string MIXER_SFX = "SFXVol";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSounds();
    }

    void Start()
    {
        // Oyun açılınca kaydedilmiş ses ayarlarını yükle
        LoadVolume();
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            // ÖNEMLİ: Sesi Mikser Grubuna Bağla
            // Sound sınıfına "AudioMixerGroup" değişkeni eklememiz gerekecek
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    // --- SES ÇALMA ---
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Ses yok: " + name);
            return;
        }
        s.source.Play();
    }

    // --- AYARLARI YÖNETME (Slider'lar burayı çağıracak) ---
    public void SetMusicVolume(float value) // Slider 0 ile 1 arası değer verir
    {
        // Mikser logaritmik çalışır (-80dB ile 0dB arası). 
        // 0-1 arasını logaritmik dB'ye çeviriyoruz.
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;

        mainMixer.SetFloat(MIXER_MUSIC, volume);
        PlayerPrefs.SetFloat("MusicVolumeSave", value); // Ham değeri (0-1) kaydet
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(MIXER_SFX, volume);
        PlayerPrefs.SetFloat("SFXVolumeSave", value);
    }

    private void LoadVolume()
    {
        // Kayıtlı ayarı çek, yoksa 0.75f varsayılan olsun
        float musicVol = PlayerPrefs.GetFloat("MusicVolumeSave", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolumeSave", 0.75f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public bool loop;
    public AudioMixerGroup mixerGroup; // YENİ: Buraya Music veya SFX grubu sürüklenecek

    [HideInInspector]
    public AudioSource source;
}