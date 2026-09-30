using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void AmmoTutorial()
    {
        SceneManager.LoadScene("AmmoTutorial");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}