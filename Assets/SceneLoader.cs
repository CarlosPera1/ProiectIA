using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void openColorScene()
    {
        SceneManager.LoadScene("ColorScene");
    }

    public void openBlobScene()
    {
        SceneManager.LoadScene("BlobScene");
    }

    public void openMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void openJumpScene()
    {
        SceneManager.LoadScene("JumpScene");
    }

    public void exitApp() 
    {
        Application.Quit();
    }
}
