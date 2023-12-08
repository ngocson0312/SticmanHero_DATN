using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame.sdk;

public class GameAdsHelperBridge : MonoBehaviour
{
    public static GameAdsHelperBridge Instance;

    public static event Action<int, string> CBRequestGDPR = null;
    private static bool isFirst = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.name = "GameAdsHelperBridge";           //Change the GameObject name to IronSourceEvents.
        }
        else
        {
            //if (this != Instance) Destroy(gameObject);
        }
    }
    public void showOpenAd(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            _showOpenAds(description);
        });
    }
    public void showOpenAdsRe(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            _showOpenAdsRe(description);
        });
    }

    public void onShowOpenNative(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            _onShowOpenNative(description);
        });
    }

    public void onOpenAdPaidEvent(string description)
    {
        AdsProcessCB.Instance().Enqueue(() =>
        {
            string[] sarr = description.Split(new char[] { ';' });
            if (sarr != null && sarr.Length == 3)
            {
                int Precision = 0;
                int.TryParse(sarr[0], out Precision);
                long lva = 0;
                long.TryParse(sarr[2], out lva);
                string adopenid = "";
#if UNITY_IOS || UNITY_IPHONE
                adopenid = PlayerPrefs.GetString($"mem_df0_open_id", AdsHelper.Instance.OpenAdIdiOS);
#elif UNITY_ANDROID
                adopenid = PlayerPrefs.GetString($"mem_df0_open_id", AdsHelper.Instance.OpenAdIdAndroid);
#endif
                FIRhelper.logEventAdsPaidAdmob(3, adopenid, Precision, sarr[1], lva);
            }
        });
    }

    void _onShowOpenNative(string description)
    {
        Debug.Log("mysdk: call _onShowOpenNative=" + description);
        if (description != null)
        {
            if (description.CompareTo("onAdClose") == 0)
            {
                FIRhelper.logEvent("show_ads_open_" + description);
                // FIRhelper.logEvent("show_ads_full_open_native");
                // FIRhelper.logEvent("show_ads_total");
                // AdsHelper.logCountTotal();
            }
            else if (description.CompareTo("onFirstAdOpen") == 0)
            {
                SDKManager.Instance.onFirstAdShow();
            }
        }
    }
    private void _showOpenAds(string description)
    {
        Debug.Log("mysdk: call _showOpenAds=" + description);
        bool isAdsShowing = false;
        bool isAllowShow = true;
        if (AdsHelper.Instance != null && AdsHelper.Instance.isAdsShowing)
        {
            isAdsShowing = true;
        }
        if (GameHelper.Instance != null)
        {
            isAllowShow = GameHelper.Instance.isAlowShowOpen;
            if (!GameHelper.Instance.isAlowShowOpen)
            {
                GameHelper.Instance.isAlowShowOpen = true;
            }
        }
        Debug.Log("mysdk: _showOpenAds isAdsShowing=" + isAdsShowing);
        if (isAllowShow && !isAdsShowing && AdsHelper.Instance != null && AdsHelper.Instance.isShowOpenAds(false) > 0)
        {
            bool isshow = AdsHelper.Instance.showOpenAd((statusShow) =>
            {
                if (statusShow == AD_State.AD_CLOSE)
                {
                    SDKManager.Instance.flagTimeScale = 0;
                    Time.timeScale = 1;
                    if (SDKManager.Instance.CBPauseGame != null)
                    {
                        SDKManager.Instance.CBPauseGame.Invoke(false);
                    }
                }
            });
            if (!isshow)
            {
                isshow = AdsHelper.Instance.showFull(isFirst, 99, false, false, "OpenAds", true, false, (satead) =>
                {
                    if (satead == AD_State.AD_CLOSE || satead == AD_State.AD_SHOW_FAIL)
                    {
                        SDKManager.Instance.flagTimeScale = 0;
                        Time.timeScale = 1;
                        if (SDKManager.Instance.CBPauseGame != null)
                        {
                            SDKManager.Instance.CBPauseGame.Invoke(false);
                        }
                    }
                });
                if (isshow)
                {
                    SDKManager.Instance.flagTimeScale = 1;
                    Time.timeScale = 0;
                    if (SDKManager.Instance.CBPauseGame != null)
                    {
                        SDKManager.Instance.CBPauseGame.Invoke(true);
                    }
                }
                else
                {
                    if (AdsHelper.Instance.currConfig.OpenAdShowtype == 3)
                    {
#if UNITY_IOS || UNITY_IPHONE
                        GameHelperIos.showOpenAds(false);
#elif UNITY_ANDROID
                        mygame.plugin.Android.GameHelperAndroid.showAppOpenAd(false);
#endif
                    }
                }
            }
            else
            {
                SDKManager.Instance.flagTimeScale = 1;
                Time.timeScale = 0;
                if (SDKManager.Instance.CBPauseGame != null)
                {
                    SDKManager.Instance.CBPauseGame.Invoke(true);
                }
            }
            isFirst = false;
        }
    }

    private void _showOpenAdsRe(string description)
    {
        Debug.Log("mysdk: call _showOpenAdsRe=" + description);
        int isshowre = 2;
        if (AdsHelper.Instance != null)
        {
            isshowre = AdsHelper.Instance.currConfig.OpenAdShowtype;
        }
        if (isshowre == 2)
        {
            _showOpenAds(description);
        }
    }
    public void requestIDFACallBack(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge requestIDFACallBack=" + description);
        if (description != null && description.Contains("3"))
        {
            PlayerPrefs.SetInt("user_allow_track_ads", 1);
        }
        else if (description != null && !description.Contains("3"))
        {
            PlayerPrefs.SetInt("user_allow_track_ads", -1);
        }
        AdsHelper.Instance.initAds();
    }

    public void gameIsBecomeActive(string description)
    {
        Debug.Log("GameAdsHelperBridge gameIsBecomeActive");
        if (AdsHelper.Instance != null)
        {
            if (AdsHelper.Instance.statuschekAdsFullErr == 2)
            {
                AdsHelper.Instance.statuschekAdsFullErr = 2;
                Invoke("waitCheckAdsCloseErr", 2);
            }
            if (AdsHelper.Instance.statuschekAdsGiftErr == 2)
            {
                AdsHelper.Instance.statuschekAdsGiftErr = 2;
                Invoke("waitCheckAdsCloseErr", 2);
            }
        }
    }

    private void waitCheckAdsCloseErr()
    {
        Debug.Log("GameAdsHelperBridge waitCheckAdsCloseErr");
        AdsHelper.Instance.waitStatusAdsCloseErr();
    }

    public void gameIsResignActive(string description)
    {
        Debug.Log("GameAdsHelperBridge gameIsResignActive");
        if (AdsHelper.Instance != null)
        {
            if (AdsHelper.Instance.statuschekAdsFullErr == 1)
            {
                AdsHelper.Instance.statuschekAdsFullErr = 2;
            }
            if (AdsHelper.Instance.statuschekAdsGiftErr == 1)
            {
                AdsHelper.Instance.statuschekAdsGiftErr = 2;
            }
        }
    }

    private void onshowCmp()
    {
        PlayerPrefs.SetInt("mem_show_CMP", 1);
    }

    public void AndroidCBOnShowCMP(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge AndroidCBOnShowCMP");
        onshowCmp();
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(0, description);
        }
    }

    public void AndroidCBCMP(string description)
    {
        if (description != null && description.Length > 0)
        {
            byte[] bytes = Encoding.Default.GetBytes(description);
            description = Encoding.UTF8.GetString(bytes);
        }
        Debug.Log("mysdk: GameAdsHelperBridge AndroidCBCMP=" + description);
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(1, description);
        }
    }

    public void iOSCBOnShowCMP(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge iOSCBOnShowCMP");
        onshowCmp();
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(0, description);
        }
    }

    public void iOSCBCMP(string description)
    {
        if (description != null && description.Length > 0)
        {
            byte[] bytes = Encoding.Default.GetBytes(description);
            description = Encoding.UTF8.GetString(bytes);
        }
        Debug.Log("mysdk: GameAdsHelperBridge iOSCBCMP=" + description);
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(1, description);
        }
        iOSCBOnShowCMPFinish("iOSCBCMP: " + description);
    }
    public void iOSCBOnShowCMPFinish(string description)
    {
        Debug.Log("mysdk: GameAdsHelperBridge iOSCBOnShowCMPFinish=" + description);
        if (CBRequestGDPR != null)
        {
            CBRequestGDPR(2, description);
        }
    }

    public void AndroidCBPrCheck(string des)
    {
        if (des != null && des.Length > 0)
        {
            AdsProcessCB.Instance().Enqueue(() =>
            {
                Debug.Log("mysdk: GameAdsHelperBridge AndroidCBPrCheck=" + des);
                PlayerPrefsBase.Instance().setInt("mem_kt_jvpirakt", 1);
                FIRhelper.logEvent($"game_invalid2");
                int rsesss = PlayerPrefsBase.Instance().getInt("mem_procinva_gema", 3);
                if (rsesss != 1 && rsesss != 2 && rsesss != 3 && rsesss != 101 && rsesss != 102 && rsesss != 103 && rsesss != 1985)
                {
                    rsesss = 103;
                }
                if (rsesss == 102 || rsesss == 103)
                {
                    SDKManager.Instance.showNotAllowGame();
                    FIRhelper.logEvent($"game_invalid2_notallow");
                }
            });
        }
    }
}