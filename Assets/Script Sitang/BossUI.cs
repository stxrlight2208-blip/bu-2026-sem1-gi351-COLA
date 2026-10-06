using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public static BossUI instance;

    [Header("Boss UI")]
    public TextMeshProUGUI bossStatusText;

    private Enemy bossEnemy;

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

        // เริ่มเกมให้ซ่อน UI
        if (bossStatusText != null)
        {
            bossStatusText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        WaveSpawner spawner = FindObjectOfType<WaveSpawner>();

        // ไม่มี WaveSpawner
        if (spawner == null)
        {
            HideBossUI();
            return;
        }

        // =========================================
        // ถ้าไม่ใช่ Wave ที่มี Boss → ซ่อน UI
        // =========================================

        int currentWave = spawner.GetCurrentWave();

        if (currentWave < 5 || currentWave % 5 != 0)
        {
            HideBossUI();
            return;
        }

        // =========================================
        // ถ้าเป็น Boss Wave แต่ยังไม่มี Boss
        // =========================================

        if (bossEnemy == null)
        {
            return;
        }

        // Boss ตายแล้ว
        if (!bossEnemy.gameObject.activeInHierarchy)
        {
            HideBossUI();
            return;
        }

        // =========================================
        // Boss LOCK / UNLOCK
        // =========================================

        if (spawner.IsBossUnlocked())
        {
            bossStatusText.gameObject.SetActive(true);

            bossStatusText.text = "BOSS UNLOCKED";
            bossStatusText.color = Color.yellow;
        }
        else
        {
            bossStatusText.gameObject.SetActive(true);

            bossStatusText.text = "BOSS LOCKED";
            bossStatusText.color = Color.red;
        }
    }

    // =========================================
    // SHOW BOSS
    // =========================================

    public void ShowBoss(Enemy boss)
    {
        if (boss == null)
            return;

        bossEnemy = boss;

        if (bossStatusText != null)
        {
            bossStatusText.gameObject.SetActive(true);

            bossStatusText.text = "BOSS LOCKED";
            bossStatusText.color = Color.red;
        }
    }

    // =========================================
    // HIDE BOSS UI
    // =========================================

    public void HideBossUI()
    {
        bossEnemy = null;

        if (bossStatusText != null)
        {
            bossStatusText.gameObject.SetActive(false);
        }
    }
}