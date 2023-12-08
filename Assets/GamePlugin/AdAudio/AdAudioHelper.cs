using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdAudioHelper : MonoBehaviour
{
    public static AdAudioHelper Instance = null;

    public List<BaseAdAudio> listAdRes;
    public Dictionary<int, BaseAdAudio> listAdUse { get; set; }

    long tShowAd = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            listAdUse = new Dictionary<int, BaseAdAudio>();

            foreach (var ad in listAdRes)
            {
                listAdUse.Add((int)ad.typeAdAudio, ad);
            }

            foreach (var ad in listAdUse)
            {
                ad.Value.onAwake();
            }
            GameAdsHelperBridge.CBRequestGDPR += onShowCmp;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != Instance) Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var ad in listAdUse)
        {
            ad.Value.onStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
#if ENABLE_ADAUDIO
        foreach (var ad in listAdUse)
        {
            ad.Value.onUpdate();
        }
#endif
    }

    private void onShowCmp(int state, string des)
    {
        if (state == 0)
        {
            onShowCmpNative();
        }
        else if (state == 1)
        {
            if (des != null && des.Length > 5)
            {
                onCMPOK(des);
            }
        }
    }

    private void onShowCmpNative()
    {
        foreach (var ad in listAdUse)
        {
            ad.Value.onShowCmpNative();
        }
    }

    private void onCMPOK(string iABTCv2String)
    {
        PlayerPrefs.SetString("mem_iab_tcv2", iABTCv2String);
        foreach (var ad in listAdUse)
        {
            ad.Value.onCMPOK(iABTCv2String);
        }
    }
    //-----------------
    public static void show(Canvas cv, RectTransform rectTf)
    {
        int countIntershow = PlayerPrefs.GetInt("count_full_4_point", 0);
        int cfco = PlayerPrefs.GetInt("cf_adaudio_cofull", 3);
        Debug.Log($"mysdk: adau AdAudioHelper show cofuu={countIntershow}, cf={cfco}");
        if (Instance != null && Instance.isOverTimeShow() && PlayerPrefs.GetInt("cf_adaudio_enable", 1) == 1 && countIntershow > cfco)
        {
            foreach (var ad in Instance.listAdUse)
            {
                ad.Value.show(cv, rectTf);
            }
        }
        else
        {
            if (Instance != null)
            {
                Debug.Log($"mysdk: adau AdAudioHelper show cf time={Instance.isOverTimeShow()}");
            }
        }
    }
    public static void show(int xOffset, int yOffset, int size)
    {
        int countIntershow = PlayerPrefs.GetInt("count_full_4_point", 0);
        int cfco = PlayerPrefs.GetInt("cf_adaudio_cofull", 3);
        Debug.Log($"mysdk: adau AdAudioHelper show cofuu={countIntershow}, cf={cfco}");
        if (Instance != null && Instance.isOverTimeShow() && PlayerPrefs.GetInt("cf_adaudio_enable", 1) == 1 && countIntershow > cfco)
        {
            foreach (var ad in Instance.listAdUse)
            {
                ad.Value.show(xOffset, yOffset, size);
            }
        }
        else
        {
            if (Instance != null)
            {
                Debug.Log($"mysdk: adau AdAudioHelper show cf time={Instance.isOverTimeShow()}");
            }
        }
    }

    public static void hide()
    {
        if (Instance != null)
        {
            foreach (var ad in Instance.listAdUse)
            {
                ad.Value.hideAds();
            }
        }
    }

    public static void close()
    {
        if (Instance != null)
        {
            foreach (var ad in Instance.listAdUse)
            {
                ad.Value.close();
            }
        }
    }

    //

    bool isOverTimeShow()
    {
        long tcurr = mygame.sdk.SdkUtil.CurrentTimeMilis();
        long dtcf = PlayerPrefs.GetInt("cf_adaudio_deltatime", 75);
        if ((tcurr - tShowAd) >= dtcf * 1000)
        {
            return true;
        }
        return false;
    }

    public void onShowAd(BaseAdAudio ad)
    {
        tShowAd = mygame.sdk.SdkUtil.CurrentTimeMilis();
    }

    public void onCloseAd(BaseAdAudio ad)
    {
        tShowAd = mygame.sdk.SdkUtil.CurrentTimeMilis();
    }    
}
