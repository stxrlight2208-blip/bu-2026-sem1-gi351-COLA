using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public Slider normalAmmoBar;
    public TMP_Text normalAmmoText;

    public Slider silverAmmoBar;
    public TMP_Text silverAmmoText;

    public int maxNormalAmmo = 30;
    public int maxSilverAmmo = 30;

    public void UpdateAmmo(int normalAmmo, int silverAmmo)
    {
        if (normalAmmoBar != null)
        {
            normalAmmoBar.maxValue = maxNormalAmmo;
            normalAmmoBar.value = normalAmmo;
        }

        if (normalAmmoText != null)
        {
            normalAmmoText.text = normalAmmo.ToString();
        }

        if (silverAmmoBar != null)
        {
            silverAmmoBar.maxValue = maxSilverAmmo;
            silverAmmoBar.value = silverAmmo;
        }

        if (silverAmmoText != null)
        {
            silverAmmoText.text = silverAmmo.ToString();
        }
    }
}