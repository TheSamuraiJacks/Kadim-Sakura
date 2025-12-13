using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrControllerRoot : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float sprintSpeed = 3f;
    public float gravity = -9.8f;
    public float jumpHeight = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Sound Settings")]
    public AudioClip walkSound;
    public AudioClip runSound;
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.3f;
    public bool playMusicOnStart = true;

    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    public Animator animator;

    [SerializeField] private Rigidbody rb;
    [SerializeField] float mouseSensitivity = 500f;
    float xRotation = 0;
    public Transform followTarget;
    public float rotPower = 10f;
    public bool isMoving;

    // Ses için AudioSource
    private AudioSource footstepAudioSource;
    private AudioSource musicAudioSource;
    private bool wasMoving = false;
    private bool wasRunning = false;

    private void Start()
    {
        // Yürüme sesleri için AudioSource oluþtur
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.loop = true;
        footstepAudioSource.volume = footstepVolume;
        footstepAudioSource.spatialBlend = 0f; // 2D ses (oyuncu için)

        // Arka plan müziði için AudioSource oluþtur
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.spatialBlend = 0f; // 2D ses
        musicAudioSource.playOnAwake = false;

        // Müziði baþlat
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }

    private void Update()
    {
        GroundCheck();
        MovePlayer();

        // Zýplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void GroundCheck()
    {
        // Eðer yer ile temas varsa aþaðý doðru hýz sýfýrlanýr
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            isMoving = true;
            Rotation();
        }
        else
        {
            isMoving = false;
        }

        // Koþma kontrolü
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("isRunning", isRunning);
        animator.SetFloat("XSpeed", x);
        animator.SetFloat("ZSpeed", z);

        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance, groundMask))
        {
            float distance = hit.distance;
            animator.SetFloat("GroundDistance", distance);
        }

        // *** YÜRÜYüÞ VE KOÞU SESLERÝNÝ KONTROL ET ***
        HandleFootstepSounds(isMoving, isRunning);

        // Yerçekimi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Yürüyüþ ve koþu seslerini yönetir
    void HandleFootstepSounds(bool moving, bool running)
    {
        // Sadece yerdeyken ses çal
        if (!isGrounded)
        {
            StopFootstepSound();
            return;
        }

        // Hareket durumu deðiþtiyse
        if (moving && !wasMoving)
        {
            // Hareket baþladý - ses baþlat
            if (running)
            {
                PlayFootstepSound(runSound, 1.2f); // Koþma biraz daha hýzlý
            }
            else
            {
                PlayFootstepSound(walkSound, 1f);
            }
        }
        else if (!moving && wasMoving)
        {
            // Hareket durdu - sesi durdur
            StopFootstepSound();
        }
        else if (moving && (running != wasRunning))
        {
            // Yürümeden koþmaya veya tersi geçiþ
            if (running)
            {
                PlayFootstepSound(runSound, 1.2f);
            }
            else
            {
                PlayFootstepSound(walkSound, 1f);
            }
        }

        wasMoving = moving;
        wasRunning = running;
    }

    // Yürüyüþ sesini baþlatýr
    void PlayFootstepSound(AudioClip clip, float pitch)
    {
        if (clip == null) return;

        if (footstepAudioSource.clip != clip)
        {
            footstepAudioSource.clip = clip;
            footstepAudioSource.pitch = pitch;
            footstepAudioSource.Play();
        }
        else if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.pitch = pitch;
            footstepAudioSource.Play();
        }
    }

    // Yürüyüþ sesini durdurur
    void StopFootstepSound()
    {
        if (footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("isJumped");

            // Zýplarken yürüme sesini durdur
            StopFootstepSound();

            if (AudioManager.instance != null)
            {
                AudioManager.instance.Play("Jump");
            }
        }
    }

    public void Rotation()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);

        // followTarget ayarý
        followTarget.transform.rotation = Quaternion.Lerp(followTarget.transform.rotation, transform.rotation, Time.deltaTime * rotPower);
    }

    // *** ARKA PLAN MÜZÝÐÝ FONKSÝYONLARI ***

    // Arka plan müziðini baþlat
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicAudioSource.isPlaying)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.Play();
        }
    }

    // Müziði durdur
    public void StopBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    // Müziði duraklat
    public void PauseBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }

    // Müziði devam ettir
    public void ResumeBackgroundMusic()
    {
        if (!musicAudioSource.isPlaying && musicAudioSource.time > 0)
        {
            musicAudioSource.UnPause();
        }
    }

    // Müzik sesini deðiþtir
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = musicVolume;
    }
}