using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public GameObject pauseMenu;
    public GameObject[] gameUIElements;
    public GameObject inventory;

    public void PauseMenu() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        pauseMenu.SetActive(true);
        foreach(GameObject go in gameUIElements) {
            go.SetActive(false);
        }

        inventory.SetActive(false);
    }

    public void ContinueGame() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        pauseMenu.SetActive(false);
        foreach(GameObject go in gameUIElements) {
            go.SetActive(true);
        }
    }

    public void SaveGame() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        SaveSystem.SaveWorld(World.Instance.worldData);
    }

    public void MainMenu() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
