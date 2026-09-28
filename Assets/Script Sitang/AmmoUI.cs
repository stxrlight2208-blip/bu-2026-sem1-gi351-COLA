using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public Slider ammoBar;
    public TMP_Text ammoText;

    public int maxAmmo = 30;
    private int currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    public void UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }
    }

    void UpdateAmmoUI()
    {
        ammoBar.maxValue = maxAmmo;
        ammoBar.value = currentAmmo;
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }
}