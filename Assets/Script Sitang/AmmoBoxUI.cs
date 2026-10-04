using UnityEngine;
using TMPro;

public class AmmoBoxUI : MonoBehaviour
{
    public static AmmoBoxUI instance;

    [Header("UI")]
    public TextMeshProUGUI ammoBoxText;

    private int currentAmmoBoxes = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    // เรียกตอน Spawn กล่องกระสุน
    public void AddAmmoBox()
    {
        currentAmmoBoxes++;
        UpdateUI();
    }

    // เรียกตอน Player เก็บกล่องกระสุน
    public void RemoveAmmoBox()
    {
        if (currentAmmoBoxes > 0)
        {
            currentAmmoBoxes--;
        }

        UpdateUI();
    }

    // ตั้งจำนวนโดยตรง
    public void SetAmmoBoxes(int amount)
    {
        currentAmmoBoxes = Mathf.Max(0, amount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ammoBoxText != null)
        {
            ammoBoxText.text = "Ammo Boxes: " + currentAmmoBoxes;
        }
    }
}