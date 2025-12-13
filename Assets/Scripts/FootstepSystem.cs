using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class FootstepSystem : MonoBehaviour
{
    [Header("Ses Dosyaları")]
    public AudioClip[] walkClips;
    public AudioClip[] sprintClips;

    [Header("Zamanlama")]
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    [Header("Ayarlar")]
    [Range(0.8f, 1.2f)] public float pitchRange = 0.1f;
    [Range(0.9f, 1.1f)] public float volumeRange = 0.1f;

    private CharacterController cc;
    private AudioSource audioSource;
    private float stepTimer;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        // 1. Oyuncu tuşlara basıyor mu? (Input Kontrolü)
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        // Vektör büyüklüğü 0 ise oyuncu duruyordur.
        float inputMagnitude = new Vector2(inputX, inputZ).magnitude;

        // EĞER: Karakter yerdeyse VE Tuşlara basılıyorsa (Hareket var)
        if (cc.isGrounded && inputMagnitude > 0.1f)
        {
            // Koşuyor mu? (Shift basılı mı?)
            // Burada çakışma olamaz çünkü bu bir "Durum" kontrolüdür.
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);

            // Hangi aralığı kullanacağız?
            float currentInterval = isSprinting ? sprintStepInterval : walkStepInterval;

            // Zamanlayıcıyı düşür
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                // SADECE BİRİ ÇALABİLİR
                if (isSprinting)
                {
                    PlayRandomSound(sprintClips);
                }
                else
                {
                    PlayRandomSound(walkClips);
                }

                stepTimer = currentInterval; // Süreyi sıfırla
            }
        }
        else
        {
            // Hareket yoksa zamanlayıcıyı sıfırla.
            // Böylece yürümeye başladığın AN (süre beklemeden) ilk adım sesi gelir.
            stepTimer = 0;
        }
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);

        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.volume = 1f + Random.Range(-volumeRange, volumeRange);

        // OneShot kullanıyoruz, Loop kapalı olmalı!
        audioSource.PlayOneShot(clips[randomIndex]);
    }
}