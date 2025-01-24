using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{


   private GameManager GameManager;
 

    private void Awake()
    {

        GameManager = GameManager.instance;
    }


    public void LoadMainMenu()
    {
    // Load the main menu scene referenced abve
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        GameManager.instance.IsInGame = false;
    }


    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        GameManager.instance.IsInGame = true;

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
