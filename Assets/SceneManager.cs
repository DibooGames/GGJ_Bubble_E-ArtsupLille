using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
 


    public void LoadMainMenu()
    {
    // Load the main menu scene referenced abve
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }


    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level");
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadScene()
    {
        GameManager.instance.ReloadStats();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }





}
