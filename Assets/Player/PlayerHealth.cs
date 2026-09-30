using UnityEngine;
using UnityEngine.UI; // ใช้สำหรับอัปเดต UI แถบเลือด (Slider)
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;      // เลือดสูงสุด
    public int currentHealth;        // เลือดปัจจุบัน

    [Header("UI Settings (Optional)")]
    public Slider healthSlider;      // ลาก UI Slider แถบเลือดมาใส่ที่นี่ (ถ้ามี)

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // ฟังก์ชันรับความเสียหาย (โดนศัตรูตี หรือโดนกระสุน)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();
        Debug.Log("Player โดนโจมตี! เลือดเหลือ: " + currentHealth);

        // ถ้าเลือดหมดให้ตาย
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ฟังก์ชันเพิ่มเลือด (เก็บกล่องพยาบาล)
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthUI();
        Debug.Log("Player เพิ่มเลือด! เลือดปัจจุบัน: " + currentHealth);
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player ตายแล้ว!");
        // ซ่อนตัวละคร หรือรีโหลดฉากใหม่
        SceneManager.LoadScene("GameOver");
    }
}