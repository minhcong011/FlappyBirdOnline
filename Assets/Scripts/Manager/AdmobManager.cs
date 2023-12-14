using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdmobManager : SingletonBase<AdmobManager>
{
#if UNITY_ANDROID || UNITY_IPHONE
#if UNITY_ANDROID
    [SerializeField] private string bannerId = "ca-app-pub-2787841785988179/9426421152";
    [SerializeField] private string interId = "ca-app-pub-2787841785988179/2184437955";
    [SerializeField] private string rewardedId = "ca-app-pub-2787841785988179/3716527969";

#elif UNITY_IPHONE
    [SerializeField]private string bannerId = "ca-app-pub-3940256099942544/2934735716";
    [SerializeField]private string interId = "ca-app-pub-3940256099942544/4411468910";
    [SerializeField]private string rewardedId = "ca-app-pub-3940256099942544/1712485313";

#endif

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;


    private void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {

            print("Ads Initialised !!");

        });
        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    #region Banner

    public void LoadBannerAd()
    {
        //create a banner
        CreateBannerView();

        //listen to banner events
        ListenToBannerEvents();

        //load the banner
        if (bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        print("Loading banner Ad !!");
        bannerView.LoadAd(adRequest);//show the banner on the screen
    }
    void CreateBannerView()
    {

        if (bannerView != null)
        {
            DestroyBannerAd();
        }
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }
    void ListenToBannerEvents()
    {
        bannerView.OnBannerAdLoaded += () =>
        {
            
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
        };
    }
    public void DestroyBannerAd()
    {

        if (bannerView != null)
        {
            print("Destroying banner Ad");
            bannerView.Destroy();
            bannerView = null;
        }
    }
    #endregion

    #region Interstitial

    public void LoadInterstitialAd()
    {

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                print("Interstitial ad failed to load" + error);
                return;
            }

            print("Interstitial ad loaded !!" + ad.GetResponseInfo());

            interstitialAd = ad;
            InterstitialEvent(interstitialAd);
        });

    }
    public bool ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            return true;
        }
        else
        {
            print("Intersititial ad not ready!!");
            return false;
        }
    }
    public void InterstitialEvent(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
           
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadInterstitialAd();
        };
    }

    #endregion

    #region Rewarded

    public void LoadRewardedAd()
    {

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                print("Rewarded failed to load" + error);
                return;
            }

            print("Rewarded ad loaded !!");
            rewardedAd = ad;
            RewardedAdEvents(rewardedAd);
        });
    }
    public void ShowRewardedAd()
    {

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {

                CompletedRewardAds();
            });
        }
        else
        {
            print("Rewarded ad not ready");
        }
    }
    public void RewardedAdEvents(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadRewardedAd();
        };
    }

    #endregion
    public void CompletedRewardAds()
    {
    }
#endif
}