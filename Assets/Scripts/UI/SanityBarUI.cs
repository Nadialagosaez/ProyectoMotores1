using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanityBarUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerSanity playerSanity;

    [Header("UI")]
    [SerializeField] private Slider sanitySlider;
    [SerializeField] private TMP_Text sanityText;

    private void Awake()
    {
        if (playerSanity != null)
        {
            playerSanity.OnSanityChanged += UpdateSanityBar;
        }
    }

    private void OnDestroy()
    {
        if (playerSanity != null)
        {
            playerSanity.OnSanityChanged -= UpdateSanityBar;
        }
    }

    private void Start()
    {
        if (playerSanity != null)
        {
            UpdateSanityBar(playerSanity.CurrentSanity, playerSanity.MaxSanity);
        }
    }

    public void UpdateSanityBar(float current, float max)
    {
        if (sanitySlider != null)
        {
            sanitySlider.maxValue = max;
            sanitySlider.value = current;
        }

        if (sanityText != null)
        {
            sanityText.text = string.Format("Cordura: {0:0}/{1:0}", current, max);
        }
    }
}
