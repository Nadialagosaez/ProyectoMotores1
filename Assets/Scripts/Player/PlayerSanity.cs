using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerSanity : MonoBehaviour
{
    public event Action<float, float> OnSanityChanged;

    [Header("Ajustes de Cordura")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity;

    public float MaxSanity => maxSanity;
    public float CurrentSanity => currentSanity;

    void Awake()
    {
        currentSanity = maxSanity;
        OnSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    public void TakeDamage(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);

        OnSanityChanged?.Invoke(currentSanity, maxSanity);

        if (currentSanity <= 0)
        {
            GameOver();
        }
       else
        {
            MessageManager.Instance.ShowDamageMessage();
        }
    }

    // Método para curar cordura (analizar si completa o solo un poco)
    public void Heal(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
        OnSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    private void GameOver()
    {
       enabled = false; 
    
        WorldSceneManager.Instance.StartCoroutine(
        WorldSceneManager.Instance.LoadSceneRoutine("GameOverScene")
    );
    }
}