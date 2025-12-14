using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    public string TagName;
    public AudioClip hitSound; // Ses dosyası
    private AudioSource audioSource;

    public float damageValue;

    void Start()
    {
        // AudioSource bileşenini al veya ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public void ChangeDamage(float dmg)
    {
        damageValue = dmg;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == TagName)
        {
            bool isEnemy = false;

            // Düşman kontrolü
            if(other.gameObject.GetComponent<Enemy>() != null)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(damageValue);
                isEnemy = true; // Sadece düşmansa true yap
            }

            // Diğer kırılabilir objeler vs.
            if(other.gameObject.GetComponent<AttackController>() != null)
            {
                other.gameObject.GetComponent<AttackController>().TakeDamage(damageValue);
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