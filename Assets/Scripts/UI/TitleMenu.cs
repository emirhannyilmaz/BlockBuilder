using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject settingsMenu;

    [Header("Settings Menu UI Elements")]
    public Slider viewDistanceSlider;
    public TextMeshProUGUI viewDistanceText;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;
    public Toggle animatedChunksToggle;

    Settings settings;

    private void Awake() {
        if(!File.Exists(Application.dataPath + "/settings.cfg")) {
            settings = new Settings();

            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);
        } else {
            string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
        }
    }

    public void StartGame() {
        VoxelData.seed = Random.Range(136454, 763231);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
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
        settings.sensitivity = sensitivitySlider.value;
        settings.animatedChunks = animatedChunksToggle.isOn;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

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