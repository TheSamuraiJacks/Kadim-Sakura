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
        // Yürüme sesleri için AudioSource oluştur
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.loop = true;
        footstepAudioSource.volume = footstepVolume;
        footstepAudioSource.spatialBlend = 0f; // 2D ses (oyuncu için)
        
        // Arka plan müziği için AudioSource oluştur
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.spatialBlend = 0f; // 2D ses
        musicAudioSource.playOnAwake = false;
        
        // Müziği başlat
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }
    
    private void Update()
    {
        GroundCheck();
        MovePlayer();
        
        // Zıplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    
    void GroundCheck()
    {
        // Eğer yer ile temas varsa aşağı doğru hız sıfırlanır
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
        
        // Koşma kontrolü
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        animator.SetBool("isRunning", isRunning);
        animator.SetFloat("XSpeed", x);
        animator.SetFloat("ZSpeed", z);
        
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance, groundMask))
        {
            float distance = hit.distance;
            animator.SetFloat("GroundDistance", distance);
        }
        
        // *** YÜRÜYüŞ VE KOŞU SESLERİNİ KONTROL ET ***
        HandleFootstepSounds(isMoving, isRunning);
        
        // Yerçekimi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    // Yürüyüş ve koşu seslerini yönetir
    void HandleFootstepSounds(bool moving, bool running)
    {
        // Sadece yerdeyken ses çal
        if (!isGrounded)
        {
            StopFootstepSound();
            return;
        }
        
        // Hareket durumu değiştiyse
        if (moving && !wasMoving)
        {
            // Hareket başladı - ses başlat
            if (running)
            {
                PlayFootstepSound(runSound, 1.2f); // Koşma biraz daha hızlı
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
            // Yürümeden koşmaya veya tersi geçiş
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
    
    // Yürüyüş sesini başlatır
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
    
    // Yürüyüş sesini durdurur
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
            
            // Zıplarken yürüme sesini durdur
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
        
        // followTarget ayarı
        followTarget.transform.rotation = Quaternion.Lerp(followTarget.transform.rotation, transform.rotation, Time.deltaTime * rotPower);
    }
    
    // *** ARKA PLAN MÜZİĞİ FONKSİYONLARI ***
    
    // Arka plan müziğini başlat
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicAudioSource.isPlaying)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.Play();
        }
    }
    
    // Müziği durdur
    public void StopBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }
    
    // Müziği duraklat
    public void PauseBackgroundMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }
    
    // Müziği devam ettir
    public void ResumeBackgroundMusic()
    {
        if (!musicAudioSource.isPlaying && musicAudioSource.time > 0)
        {
            musicAudioSource.UnPause();
        }
    }
    
    // Müzik sesini değiştir
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = musicVolume;
    }
}