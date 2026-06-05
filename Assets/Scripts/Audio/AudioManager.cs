using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip pickupItemClip;
    [SerializeField] private AudioClip pickupSanityClip;
    [SerializeField] private AudioClip textPopClip; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayPickupItem() => PlayClip(pickupItemClip);
    public void PlayPickupSanity() => PlayClip(pickupSanityClip);
    public void PlayTextPop() => PlayClip(textPopClip);

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            // PlayOneShot permite que los sonidos se superpongan si ocurren muy rápido
            sfxSource.PlayOneShot(clip); 
        }
    }
}