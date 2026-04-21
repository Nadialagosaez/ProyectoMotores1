using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
    [Header("Ajustes de Cordura")]
    public float maxSanity = 100f;
    private float currentSanity;
   

    void Awake()
    {
        currentSanity = maxSanity;
    }

    public void TakeDamage(float amount)
    {
        currentSanity -= amount;
        //intentaré poner alguna animación de corte de cámara cuando reciba daño

        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);

        Debug.Log("Cordura actual: " + currentSanity);

        if (currentSanity <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Perdiste la cordura, sos un ente más del hospital!");
        //Pantalla derrota
    }
}