using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour, IUnityAdsShowListener {
    private static AdManager instance;
    private string gameId = "4374681";
    private bool testMode = false;
    private float interstitialAdShowTime;
    public float delayBetweenInterstitialAds = 10f;
    private bool interstitialAdShowing = false;

    private void Awake() {
        if(instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        interstitialAdShowTime = Time.time;

        Advertisement.Initialize(gameId, testMode);

        Advertisement.Load("Interstitial_Android");
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load("Banner_Android");
        Advertisement.Banner.Show("Banner_Android");
    }

    void Update() {
        HandleInterstitialAd();
    }

    public void HandleInterstitialAd() {
        if(Time.time - interstitialAdShowTime >= delayBetweenInterstitialAds && !interstitialAdShowing) {
            Advertisement.Show("Interstitial_Android", this);
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) {

    }

    public void OnUnityAdsShowStart(string placementId) {
        if(placementId == "Interstitial_Android") {
            interstitialAdShowing = true;
            if(SceneManager.GetActiveScene().name == "Game") {
                World.Instance.player.gameObject.GetComponent<Player>().isPressedDown = false;
                World.Instance.player.gameObject.GetComponent<Player>().mouseHorizontal = 0f;
                World.Instance.player.gameObject.GetComponent<Player>().mouseVertical = 0f;
            }
        }
    }

    public void OnUnityAdsShowClick(string placementId) {

    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) {
        if(placementId == "Interstitial_Android") {
            Advertisement.Load("Interstitial_Android");
            interstitialAdShowTime = Time.time;
            interstitialAdShowing = false;
        }
    }
}
