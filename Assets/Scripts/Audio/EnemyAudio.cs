using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private AudioSource audioSource;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip attackClip; 
    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackClip != null)
        {
            audioSource.PlayOneShot(attackClip);
        }
    }
}