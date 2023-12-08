using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_ODEEO
#endif


public class OdeeoAd : BaseAdAudio
{
#if ENABLE_ODEEO
    AdUnit adUnit;
#endif
    int stateShowAd = 0;
    bool isAdOk = false;
    long tShowAds = 0;

    public override void onAwake()
    {
        typeAdAudio = AdAudioType.Odeeo;
    }

    public override void onStart()
    {
#if ENABLE_ODEEO
        PlayOnSDK.OnInitializationFinished += OnInitialized;
        PlayOnSDK.OnInitializationFailed += OnInitializationFailed;
        PlayOnSDK.Initialize("27e0eb92-71fc-4117-9b5a-c34db3ea84f3", AppConfig.appid);
        Debug.Log("mysdk: adau PlayOnDemo init");
#endif
    }
    //================
    void OnInitialized()
    {
        //PlayOnSDK Initialized
    }
    void OnInitializationFailed(int errorParam, string error)
    {
        //PlayOnSDK initialization Failed
    }
    //================

    public override void onShowCmpNative()
    {

    }

    public override void onCMPOK(string iABTCv2String)
    {
#if ENABLE_ODEEO
        PlayOnSDK.SetGdprConsent(true, iABTCv2String);
#endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
#if ENABLE_ODEEO
        PlayOnSDK.onApplicationPause(pauseStatus);
#endif
    }

    public override void onUpdate()
    {
#if ENABLE_ODEEO
        if (stateShowAd == 2)
        {

        }
#endif
    }

    public override BaseAdAudio show(Canvas cv, RectTransform rectTf)
    {
        Debug.Log("mysdk: adau PlayOnDemo show1");
#if ENABLE_ODEEO
        this.canvas = cv;
        this.rect = rectTf;
        if (canvas != null && rect != null)
        {
            if (adUnit == null)
            {
                PlayOnSDK.AdUnitType adType = PlayOnSDK.AdUnitType.AudioLogoAd;
                adUnit = new AdUnit(adType);
                adUnit.LinkLogoToRectTransform(PlayOnSDK.Position.Centered, rect, canvas);
                adUnit.SetProgressBar(Color.white);
                adUnit.SetActionButton(PlayOnSDK.AdUnitActionButtonType.Mute, 15f);
                adUnit.SetVisualization(Color.white, Color.cyan);

                adUnit.AdCallbacks.OnAvailabilityChanged += AdOnAvailabilityChanged;
                adUnit.AdCallbacks.OnShow += AdOnShow;
                adUnit.AdCallbacks.OnClose += AdOnClose;
                adUnit.AdCallbacks.OnClick += AdOnClick;
                //OnRewarded available only for Rewarded Ad Types.
                adUnit.AdCallbacks.OnReward += AdOnReward;
                adUnit.AdCallbacks.OnImpression += AdOnImpression;
                stateShowAd = 1;
                Debug.Log("mysdk: adau PlayOnDemo show create");
            }
            else
            {
                adUnit.LinkLogoToRectTransform(PlayOnSDK.Position.Centered, rect, canvas);
                if (isAdOk)
                {
                    Debug.Log("mysdk: adau PlayOnDemo show show");
                    stateShowAd = 2;
                    adUnit.ShowAd();
                    tShowAds = mygame.sdk.SdkUtil.CurrentTimeMilis();
                }
                else
                {
                    Debug.Log("mysdk: adau PlayOnDemo show wait");
                    stateShowAd = 1;
                }
            }
            gameObject.SetActive(true);
        }
#endif
        return this;
    }

    public override BaseAdAudio show(int xOffset, int yOffset, int size)
    {
        Debug.Log("mysdk: adau PlayOnDemo show2");
#if ENABLE_ODEEO
        if (adUnit == null)
        {
            PlayOnSDK.AdUnitType adType = PlayOnSDK.AdUnitType.AudioLogoAd;
            adUnit = new AdUnit(adType);
            adUnit.SetLogo(PlayOnSDK.Position.Centered, xOffset, yOffset, size);
            adUnit.SetProgressBar(Color.white);
            adUnit.SetActionButton(PlayOnSDK.AdUnitActionButtonType.Mute, 15f);
            adUnit.SetVisualization(Color.white, Color.cyan);

            adUnit.AdCallbacks.OnAvailabilityChanged += AdOnAvailabilityChanged;
            adUnit.AdCallbacks.OnShow += AdOnShow;
            adUnit.AdCallbacks.OnClose += AdOnClose;
            adUnit.AdCallbacks.OnClick += AdOnClick;
            //OnRewarded available only for Rewarded Ad Types.
            adUnit.AdCallbacks.OnReward += AdOnReward;
            adUnit.AdCallbacks.OnImpression += AdOnImpression;
            stateShowAd = 1;
            Debug.Log("mysdk: adau PlayOnDemo show create");
        }
        else
        {
            adUnit.SetLogo(PlayOnSDK.Position.Centered, xOffset, yOffset, size);
            if (isAdOk)
            {
                Debug.Log("mysdk: adau PlayOnDemo show show");
                stateShowAd = 2;
                adUnit.ShowAd();
                tShowAds = mygame.sdk.SdkUtil.CurrentTimeMilis();
            }
            else
            {
                Debug.Log("mysdk: adau PlayOnDemo show wait");
                stateShowAd = 1;
            }
        }
        gameObject.SetActive(true);
#endif
        return this;
    }

    private void _show()
    {

    }

    public override void hideAds()
    {
        gameObject.SetActive(false);
        stateShowAd = 0;
#if ENABLE_ODEEO
        if (adUnit != null)
        {
        }
#endif
    }

    public override void close()
    {
#if ENABLE_ODEEO
        adUnit.CloseAd();
        adUnit.Dispose();
        adUnit = null;
#endif
    }

    //========================================================
#if ENABLE_ODEEO
    public void AdOnAvailabilityChanged(bool isAdAvailable)
    {
        Debug.Log("mysdk: adau PlayOnDemo AdOnAvailabilityChanged Callback=" + isAdAvailable);
        if (isAdAvailable)
        {
            if (stateShowAd == 1)
            {
                Debug.Log("mysdk: adau PlayOnDemo AdOnAvailabilityChanged show");
                stateShowAd = 2;
                adUnit.ShowAd();
                tShowAds = mygame.sdk.SdkUtil.CurrentTimeMilis();
                isAdOk = false;
            }
            else
            {
                Debug.Log("mysdk: adau PlayOnDemo AdOnAvailabilityChanged change flag isok stateAdAvaiable=" + stateShowAd);
                isAdOk = true;
            }
        }
        else
        {
            isAdOk = false;
        }
    }
    public void AdOnClick()
    {
        Debug.Log("mysdk: adau PlayOnDemo AdOnClick Callback");
    }
    public void AdOnClose()
    {
        Debug.Log("mysdk: adau PlayOnDemo AdOnClose Callback");
        if (stateShowAd == 2)
        {
            stateShowAd = 0;
        }
        AdAudioHelper.Instance.onCloseAd(this);
    }
    public void AdOnShow()
    {
        Debug.Log("mysdk: adau PlayOnDemo AdOnShow Callback");
        AdAudioHelper.Instance.onShowAd(this);
    }
    public void AdOnReward(float amount)
    {
        Debug.Log("mysdk: adau PlayOnDemo AdOnRewarded Callback with " + amount);
    }
    public void AdOnImpression(AdUnit.ImpressionData data)
    {
        Debug.Log("mysdk: adau PlayOnDemo AdOnImpression Callback");
    }
#endif
}
