using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
    [Header("Referencias")]
    private PlayerSanity playerSanity;
    private Rigidbody rb; 

    [SerializeField] private AudioMixer mainMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource breathingSource;
    [SerializeField] private AudioSource heartbeatSource;

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip[] footstepClips; 
    [SerializeField] private AudioClip breathingClip;
    [SerializeField] private AudioClip heartbeatClip;

    [Header("Configuración de Pasos")]
    [SerializeField] private float tiempoEntrePasos = 0.5f;
    private float temporizadorPasos;

    [Header("Configuración de Alerta/Pánico")]
    [SerializeField] private float limiteCorduraAlerta = 60f; 
    [SerializeField] private float velocidadFade = 0.5f;     

    void Awake()
    {
        playerSanity = GetComponent<PlayerSanity>();
        rb = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        if (mainMixer != null) mainMixer.SetFloat("SanityCutoff", 22000f);

        if (breathingSource != null && breathingClip != null)
        {
            breathingSource.clip = breathingClip;
            breathingSource.loop = true;
            breathingSource.volume = 0.1f;
            breathingSource.Play();
        }

        if (heartbeatSource != null && heartbeatClip != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0f;
            heartbeatSource.Play();
        }
    }

    void Update()
    {
        if (!WorldSceneManager.IsGameReady) return;
        
        ControlarPasos();
        ControlarAmbienteCordura();
    }

    private void ControlarPasos()
    {
        if (rb == null || footstepClips.Length == 0) return;

        Vector3 velocidadHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        bool estaCaminando = velocidadHorizontal.magnitude > 0.1f && Mathf.Abs(rb.linearVelocity.y) < 0.1f;

        if (estaCaminando)
        {
            temporizadorPasos += Time.deltaTime;

            if (temporizadorPasos >= tiempoEntrePasos)
            {
                int indiceAzar = Random.Range(0, footstepClips.Length);
                footstepSource.PlayOneShot(footstepClips[indiceAzar]);

                temporizadorPasos = 0f; 
            }
        }
        else
        {
            temporizadorPasos = tiempoEntrePasos; 
        }
    }

    private void ControlarAmbienteCordura()
    {
        if (playerSanity == null) return;

        float corduraActual = playerSanity.CurrentSanity;
        float porcentajePanico = 1f - (corduraActual / 100f);

        // Latidos
        if (corduraActual < limiteCorduraAlerta)
        {
            float volumenObjetivoLatido = (limiteCorduraAlerta - corduraActual) / limiteCorduraAlerta;
            heartbeatSource.volume = Mathf.MoveTowards(heartbeatSource.volume, volumenObjetivoLatido, Time.deltaTime * velocidadFade);
        }
        else
        {
            heartbeatSource.volume = Mathf.MoveTowards(heartbeatSource.volume, 0f, Time.deltaTime * velocidadFade);
        }

        // respiracion
        breathingSource.pitch = Mathf.Lerp(1f, 1.5f, porcentajePanico);
        breathingSource.volume = Mathf.Lerp(0.1f, 0.9f, porcentajePanico);

        //Lowpass
        if (mainMixer != null)
        {
            float frecuenciaObjetivo = Mathf.Lerp(22000f, 600f, porcentajePanico);
            
            mainMixer.SetFloat("SanityCutoff", frecuenciaObjetivo);
        }
    }
}