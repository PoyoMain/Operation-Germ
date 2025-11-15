using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void StageSelect()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    public void Credits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MainButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
