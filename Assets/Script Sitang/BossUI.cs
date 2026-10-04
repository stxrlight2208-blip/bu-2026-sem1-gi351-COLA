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

        if (bossStatusText != null)
        {
            bossStatusText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (bossEnemy == null)
            return;

        if (!bossEnemy.gameObject.activeInHierarchy)
        {
            HideBossUI();
            return;
        }

        WaveSpawner spawner =
            FindObjectOfType<WaveSpawner>();

        if (spawner != null &&
            spawner.IsBossUnlocked())
        {
            bossStatusText.text =
                "BOSS UNLOCKED";

            bossStatusText.color =
                Color.yellow;
        }
        else
        {
            bossStatusText.text =
                "BOSS LOCKED";

            bossStatusText.color =
                Color.red;
        }
    }

    public void ShowBoss(Enemy boss)
    {
        if (boss == null)
            return;

        bossEnemy = boss;

        if (bossStatusText != null)
        {
            bossStatusText.gameObject.SetActive(true);

            bossStatusText.text =
                "BOSS LOCKED";

            bossStatusText.color =
                Color.red;
        }
    }

    public void HideBossUI()
    {
        bossEnemy = null;

        if (bossStatusText != null)
        {
            bossStatusText.gameObject.SetActive(false);
        }
    }
}