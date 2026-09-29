using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public GameObject[] characters;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
            {
                bool isActive = (i == selectedCharacter);
                characters[i].SetActive(isActive);

                // ถ้าเป็นตัวละครที่เปิดใช้งาน ให้ดึง SpriteRenderer ส่งไปให้ PlayerAimAndWeapon อัตโนมัติ
                if (isActive)
                {
                    PlayerAimAndWeapon weaponScript = GetComponent<PlayerAimAndWeapon>();
                    if (weaponScript != null)
                    {
                        SpriteRenderer sr = characters[i].GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            weaponScript.bodySprite = sr;
                        }
                    }
                }
            }
        }
    }
}