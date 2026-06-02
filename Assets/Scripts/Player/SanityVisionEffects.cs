using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityVisionEffects : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerSanity playerSanity;
    [SerializeField] private Volume globalVolume;

    [Header("Configuración del Flash e Impacto")]
    [SerializeField] private float recoverSpeed = 2.0f; 
    [SerializeField] private float hitBlindnessAmount = 4.5f;

    [Header("Efecto de Imagen Movida / Mareo")]
    [Tooltip("Velocidad del balanceo de la pantalla.")]
    [SerializeField] private float wobbleSpeed = 4.5f;
    [Tooltip("Fuerza del movimiento/vibración de la imagen.")]
    [SerializeField] private float wobbleAmount = 0.35f;

    [Header("Límites Máximos de Efectos")]
    [Range(0f, 1f)] [SerializeField] private float maxChromatic = 1.0f;       
    [Range(-1f, 1f)] [SerializeField] private float maxLensDistortion = -0.7f; 
    [Range(0f, 1f)] [SerializeField] private float maxVignette = 0.65f;       
    [Range(0f, 1f)] [SerializeField] private float maxBlur = 0.9f;

    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private MotionBlur motionBlur;

    private float targetLocura = 0f;          
    private float currentLocuraVisual = 0f;   
    private float currentFlash = 0f; 
    private float lastSanity;
    private bool isExiting = false;

    void Start()
    {
        if (globalVolume == null || globalVolume.profile == null) return;

        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out lensDistortion);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out colorAdjustments);
        globalVolume.profile.TryGet(out motionBlur);

        if (playerSanity != null)
        {
            lastSanity = playerSanity.MaxSanity;
            playerSanity.OnSanityChanged += OnSanityChanged;
            ActualizarValoresLogica(playerSanity.CurrentSanity, playerSanity.MaxSanity, false);
            currentLocuraVisual = targetLocura;
        }
    }

    void OnDestroy()
    {
        if (playerSanity != null) playerSanity.OnSanityChanged -= OnSanityChanged;
    }

    // Alerta de cambio de escena para evitar que el script toque el volumen mientras se destruye
    void OnApplicationQuit() => isExiting = true;
    void OnDisable() => isExiting = true;

    private void OnSanityChanged(float currentSanity, float maxSanity)
    {
        ActualizarValoresLogica(currentSanity, maxSanity, true);
    }

    private void ActualizarValoresLogica(float currentSanity, float maxSanity, bool puedeGenerarFlash)
    {
        // 1. Calculamos el porcentaje real (0 a 1)
        float rawLocura = 1f - (currentSanity / maxSanity);

        // 2. ¡EL TRUCO!: Elevamos a 0.5 (Raíz cuadrada). Hace que el valor se dispare al principio.
        targetLocura = Mathf.Pow(rawLocura, 0.4f); 

        // 3. Si ha recibido un zarpazo, metemos el pico de locura y ceguera
        if (puedeGenerarFlash && currentSanity < lastSanity)
        {
            currentLocuraVisual = 1f;       
            currentFlash = hitBlindnessAmount; 
        }

        lastSanity = currentSanity;
    }

    void Update()
    {
        // Si la escena se está rompiendo o el volumen ya no existe, abortamos inmediatamente
        if (isExiting || globalVolume == null) return; 

        // Suavizados basculantes
        currentLocuraVisual = Mathf.Lerp(currentLocuraVisual, targetLocura, Time.deltaTime * recoverSpeed);
        currentFlash = Mathf.Lerp(currentFlash, 0f, Time.deltaTime * recoverSpeed);

        // ONDA DE MAREO: Multiplicamos dos senos a distintas velocidades para que el movimiento sea caótico y no repetitivo
        float mareoOnda = (Mathf.Sin(Time.time * wobbleSpeed) + Mathf.Sin(Time.time * (wobbleSpeed * 1.5f))) * wobbleAmount * currentLocuraVisual;

        // Aplicamos los efectos multiplicados por la nueva curva agresiva + el mareo dinámico
        if (chromaticAberration != null)
        {
            // La aberración cromática bailará un poco con el mareo para dar efecto de "visión doble defectuosa"
            chromaticAberration.intensity.value = Mathf.Clamp01(currentLocuraVisual * maxChromatic + (Mathf.Abs(mareoOnda) * 0.5f));
        }

        if (lensDistortion != null)
        {
            // La lente se encoge y estira constantemente simulando un viaje psicodélico
            lensDistortion.intensity.value = (currentLocuraVisual * maxLensDistortion) + mareoOnda;
        }

        if (vignette != null)
        {
            vignette.intensity.value = currentLocuraVisual * maxVignette;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = currentFlash;
        }

        if (motionBlur != null)
        {
            motionBlur.intensity.value = currentLocuraVisual * maxBlur;
        }
    }
}