using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip pickupItemClip;
    [SerializeField] private AudioClip pickupSanityClip;
    [SerializeField] private AudioClip textPopClip; 
    [SerializeField] private AudioClip finalWinClip;

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
    public void PlayFinalLoop()
    {
        if (musicSource != null && finalWinClip != null)
        {
            musicSource.clip = finalWinClip;
            musicSource.loop = true; // Hacemos que se repita infinitamente
            musicSource.Play();
        }
    }

    public void PlayPickupItem() => PlayClip(pickupItemClip);
    public void PlayPickupSanity() => PlayClip(pickupSanityClip);
    public void PlayTextPop() => PlayClip(textPopClip);

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            // PlayOneShot permite que los sonidos se superpongan si ocurren muy rápido
            sfxSource.PlayOneShot(clip); 
        }
    }
 
}