using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject[] graves;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        for (int i = 0; i < graves.Length; i++)
        {
            graves[i].SetActive(false);
        }

        if (selectedCharacter >= 0 && selectedCharacter < graves.Length)
        {
            graves[selectedCharacter].SetActive(true);
        }
    }
}