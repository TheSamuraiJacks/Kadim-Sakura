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

    // Ses i�in AudioSource
    private AudioSource footstepAudioSource;
    private AudioSource musicAudioSource;
    private bool wasMoving = false;
    private bool wasRunning = false;

    private void Start()
    {
        // Y�r�me sesleri i�in AudioSource olu�tur
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.loop = true;
        footstepAudioSource.volume = footstepVolume;
        footstepAudioSource.spatialBlend = 0f; // 2D ses (oyuncu i�in)

        // Arka plan m�zi�i i�in AudioSource olu�tur
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.spatialBlend = 0f; // 2D ses
        musicAudioSource.playOnAwake = false;

        // M�zi�i ba�lat
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }

    private void Update()
    {
        GroundCheck();
        MovePlayer();

        // Z�plama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void GroundCheck()
    {
        // E�er yer ile temas varsa a�a�� do�ru h�z s�f�rlan�r
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

        // Ko�ma kontrol�
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("isRunning", isRunning);
        animator.SetFloat("XSpeed", x);
        animator.SetFloat("ZSpeed", z);

        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance, groundMask))
        {
            float distance = hit.distance;
            animator.SetFloat("GroundDistance", distance);
        }

        // *** Y�R�Y�� VE KO�U SESLER�N� KONTROL ET ***
        HandleFootstepSounds(isMoving, isRunning);

        // Yer�ekimi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Y�r�y�� ve ko�u seslerini y�netir
        // Yürüyüş ve koşu seslerini yönetir
    void HandleFootstepSounds(bool moving, bool running)
    {
        // Eğer yerdeysek ve hareket ediyorsak ses çalmalıyız
        if (isGrounded && moving)
        {
            if (running)
            {
                PlayFootstepSound(runSound, 1.2f); // Koşma
            }
            else
            {
                PlayFootstepSound(walkSound, 1f); // Yürüme
            }
        }
        // Hareket etmiyor veya havadaysak sesi durdur
        else
        {
            StopFootstepSound();
        }
        
        // wasMoving ve wasRunning değişkenlerine artık ihtiyacımız kalmadı ama
        // kodun geri kalanında hata vermemesi için güncelleyebiliriz veya silebiliriz.
        wasMoving = moving;
        wasRunning = running;
    }

    // Y�r�y�� sesini ba�lat�r
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

    // Y�r�y�� sesini durdurur
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

            // Z�plarken y�r�me sesini durdur
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

        // followTarget ayar�
        Vector3 temp = transform.eulerAngles;
        transform.rotation = Quaternion.Lerp(transform.rotation, followTarget.transform.rotation,  Time.deltaTime * rotPower);
        transform.eulerAngles = new Vector3(temp.x, transform.eulerAngles.y, temp.z);
    }

    // *** ARKA PLAN M�Z��� FONKS�YONLARI ***

    // Arka plan m�zi�ini ba�lat
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicAudioSource.isPlaying)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.Play();
        }
    }

    // M�zi�i durdur
    public void StopBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    // M�zi�i duraklat
    public void PauseBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }

    // M�zi�i devam ettir
    public void ResumeBackgroundMusic()
    {
        if (!musicAudioSource.isPlaying && musicAudioSource.time > 0)
        {
            musicAudioSource.UnPause();
        }
    }

    // M�zik sesini de�i�tir
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = musicVolume;
    }
}