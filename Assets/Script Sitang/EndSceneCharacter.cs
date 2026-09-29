using UnityEngine;
using UnityEngine.UI;

public class EndSceneCharacter : MonoBehaviour
{
    public Image characterImage;
    public Sprite[] characterSprites;

    void Start()
    {
        int selectedCharacter =
            PlayerPrefs.GetInt("SelectedCharacter", 0);

        characterImage.sprite =
            characterSprites[selectedCharacter];
    }
}