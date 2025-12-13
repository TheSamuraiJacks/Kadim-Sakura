using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    public string TagName;
    public AudioClip hitSound; // Ses dosyası
    private AudioSource audioSource;

    void Start()
    {
        // AudioSource bileşenini al veya ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == TagName)
        {
            bool isEnemy = false;

            // Düşman kontrolü
            if(other.gameObject.GetComponent<Enemy>() != null)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(10);
                isEnemy = true; // Sadece düşmansa true yap
            }

            // Diğer kırılabilir objeler vs.
            if(other.gameObject.GetComponent<AttackController>() != null)
            {
                other.gameObject.GetComponent<AttackController>().TakeDamage(1);
                // Burası isEnemy'yi true YAPMAZ, böylece ses çalmaz
            }

            // Sadece düşmana vurduysak ses çal
            if (isEnemy && audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }
}