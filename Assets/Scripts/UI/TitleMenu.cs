using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

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

    bool enteredSettingsMenu = false;

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
        Advertisement.Banner.Show("Banner_Android");
        resetWorldButton.SetActive(SaveSystem.CheckIfWorldExists("New World"));
    }

    public void StartGame() {
        Advertisement.Banner.Hide();
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ResetWorld() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        SaveSystem.ResetWorld("New World");
        resetWorldButton.SetActive(false);
    }

    public void EnterSettings() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        viewDistanceSlider.value = settings.viewDistance;
        UpdateViewDistanceSlider();
        sensitivitySlider.value = settings.sensitivity;
        UpdateSensitivitySlider();
        animatedChunksToggle.isOn = settings.animatedChunks;
        enteredSettingsMenu = true;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void LeaveSettings() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        settings.viewDistance = (int) viewDistanceSlider.value;
        settings.loadDistance = settings.viewDistance * 2;
        settings.sensitivity = sensitivitySlider.value;
        settings.animatedChunks = animatedChunksToggle.isOn;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "settings.cfg", jsonExport);

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        enteredSettingsMenu = false;
    }

    public void QuitGame() {
        FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        Application.Quit();
    }

    public void UpdateViewDistanceSlider() {
        viewDistanceText.SetText("View Distance: " + viewDistanceSlider.value);
    }

    public void UpdateSensitivitySlider() {
        sensitivityText.SetText("Sensitivity: " + sensitivitySlider.value.ToString("F1"));
    }

    public void AnimatedChunksToggle() {
        if(enteredSettingsMenu) {
            FindObjectOfType<SoundManager>().PlaySound("ButtonClickSound", 1.0f);
        }
    }
}
