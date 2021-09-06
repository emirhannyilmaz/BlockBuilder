using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public GameObject pauseMenu;
    public GameObject[] gameUIElements;

    public void PauseMenu() {
        pauseMenu.SetActive(true);
        foreach(GameObject go in gameUIElements) {
            go.SetActive(false);
        }
    }

    public void ContinueGame() {
        pauseMenu.SetActive(false);
        foreach(GameObject go in gameUIElements) {
            go.SetActive(true);
        }
    }

    public void SaveGame() {
        SaveSystem.SaveWorld(World.Instance.worldData);
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
