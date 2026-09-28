using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public GameObject[] characters;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == selectedCharacter);
        }
    }
}
