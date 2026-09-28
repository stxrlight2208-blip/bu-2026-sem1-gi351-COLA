using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public Animator[] characters;

    void Start()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].enabled = false;
        }
    }

    public void SelectCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].enabled = false;
        }

        characters[index].enabled = true;
        characters[index].Play("Character_Run");
    }
}