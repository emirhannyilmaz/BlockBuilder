using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour {
    public static string appPath;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject loadingScreen;
    public GameObject resetWorldButton;

    [Header("Settings Menu UI Elements")]
    public Slider viewDistanceSlider;
    public TextMeshProUGUI viewDistanceText;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;
    public Toggle animatedChunksToggle;

    Settings settings;

    private void Awake() {
        appPath = Application.persistentDataPath;

        if(!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "settings.cfg")) {
            settings = new Settings();

            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "settings.cfg", jsonExport);
        } else {
            string jsonImport = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
        }
    }

    private void Start() {
        resetWorldButton.SetActive(SaveSystem.CheckIfWorldExists("New World"));
    }

    public void StartGame() {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ResetWorld() {
        SaveSystem.ResetWorld("New World");
        resetWorldButton.SetActive(false);
    }

    public void EnterSettings() {
        viewDistanceSlider.value = settings.viewDistance;
        UpdateViewDistanceSlider();
        sensitivitySlider.value = settings.sensitivity;
        UpdateSensitivitySlider();
        animatedChunksToggle.isOn = settings.animatedChunks;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void LeaveSettings() {
        settings.viewDistance = (int) viewDistanceSlider.value;
        settings.loadDistance = settings.viewDistance * 2;
        settings.sensitivity = sensitivitySlider.value;
        settings.animatedChunks = animatedChunksToggle.isOn;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "settings.cfg", jsonExport);

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void UpdateViewDistanceSlider() {
        viewDistanceText.SetText("View Distance: " + viewDistanceSlider.value);
    }

    public void UpdateSensitivitySlider() {
        sensitivityText.SetText("Sensitivity: " + sensitivitySlider.value.ToString("F1"));
    }
}
