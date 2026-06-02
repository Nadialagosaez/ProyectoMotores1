using UnityEngine;
using TMPro;
using System.Collections;

public class ClockLoop : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    public int horaInicial = 12;
    public int minutoInicial = 0;
    [Tooltip("Multiplicador de velocidad. 60 significa que 1 segundo real es 1 minuto en el juego.")]
    public float multiplicadorVelocidad = 60f;
    
    [Header("Configuración de Fallas")]
    [Range(0f, 1f)] public float probabilidadFalla = 0.15f;
    public float duracionFallaMax = 0.5f;

    [Header("Componentes")]
    public TextMeshPro textoReloj;

    private float tiempoTranscurrido;
    private bool estaFallando = false;

    void Start()
    {
        ReiniciarReloj();
    }

    void Update()
    {
        // El tiempo avanza multiplicado por la velocidad elegida
        tiempoTranscurrido += Time.deltaTime * multiplicadorVelocidad;

        // Si el reloj está en medio de un glitch, el bucle de renderizado se detiene temporalmente
        if (estaFallando) return;

        // Intento aleatorio de generar una falla visual
        if (Random.value < probabilidadFalla * Time.deltaTime)
        {
            StartCoroutine(EfectoMalFuncionamiento());
        }
        else
        {
            ActualizarPantallaNormal();
        }
    }

    void ActualizarPantallaNormal()
    {
        int segundosTotales = Mathf.FloorToInt(tiempoTranscurrido);
        int segundos = segundosTotales % 60;
        int minutosTotales = minutoInicial + (segundosTotales / 60);
        int minutos = minutosTotales % 60;
        int horas = (horaInicial + (minutosTotales / 60)) % 24;

        textoReloj.text = string.Format("{0:00}:{1:00}:{2:00}", horas, minutos, segundos);
    }

    // Corrutina que simula el mal funcionamiento analógico/digital
    IEnumerator EfectoMalFuncionamiento()
    {
        estaFallando = true;
        float tiempoFalla = Random.Range(0.1f, duracionFallaMax);
        float relojInterno = 0f;

        while (relojInterno < tiempoFalla)
        {
            // Elige un efecto de falla al azar
            int tipoGlitch = Random.Range(0, 3);

            if (tipoGlitch == 0)
            {
                // Efecto 1: Pantalla completamente apagada (Vacía)
                textoReloj.text = "        ";
            }
            else if (tipoGlitch == 1)
            {
                // Efecto 2: Caracteres corruptos o estáticos
                textoReloj.text = "--:--:--";
            }
            else
            {
                // Efecto 3: Números locos y aleatorios
                textoReloj.text = string.Format("{0:00}:{1:00}:{2:00}", Random.Range(0, 99), Random.Range(0, 99), Random.Range(0, 99));
            }

            // Velocidad del parpadeo interno de la falla
            yield return new WaitForSeconds(0.05f);
            relojInterno += 0.05f;
        }

        estaFallando = false;
    }

    public void ReiniciarReloj()
    {
        tiempoTranscurrido = 0f;
        estaFallando = false;
        ActualizarPantallaNormal();
    }
}
