using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Placement;
using SimpleInputNamespace;

public class AdManager : MonoBehaviour {
    private static AdManager instance;
    private InterstitialAdGameObject interstitialAd;
    private float interstitialAdShowTime;
    public float delayBetweenInterstitialAds = 10f;
    [HideInInspector]
    public bool interstitialAdShowing = false;

    private void Awake() {
        if(instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        interstitialAdShowTime = Time.time;

        MobileAds.Initialize((initStatus) => {
            interstitialAd = MobileAds.Instance.GetAd<InterstitialAdGameObject>("Interstitial Ad");
            interstitialAd.LoadAd();
        });
    }

    private void Update() {
        HandleInterstitialAd();
    }

    public void HandleInterstitialAd() {
        if(Time.time - interstitialAdShowTime >= delayBetweenInterstitialAds && !interstitialAdShowing) {
            interstitialAd.ShowIfLoaded();
        }
    }

    public void OnInterstitialAdOpening() {
        interstitialAdShowing = true;
        if(SceneManager.GetActiveScene().name == "Game") {
            World.Instance.player.gameObject.GetComponent<Player>().isPressedDown = false;
            World.Instance.player.gameObject.GetComponent<Player>().mouseHorizontal = 0f;
            World.Instance.player.gameObject.GetComponent<Player>().mouseVertical = 0f;
        }
    }

    public void OnInterstitialAdClosed() {
        interstitialAd.LoadAd();
        interstitialAdShowTime = Time.time;
        interstitialAdShowing = false;
    }
}
