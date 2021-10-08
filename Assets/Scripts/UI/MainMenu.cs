using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame(int gridSize)
    {
        PlayerPrefs.SetInt("gridSize", gridSize);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
