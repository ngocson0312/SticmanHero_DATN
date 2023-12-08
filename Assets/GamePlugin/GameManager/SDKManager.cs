//#define CHECK_4INAPP
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System.Globalization;

namespace mygame.sdk
{
    public delegate void TakeScreenShootCallback(Texture2D texture);

    public enum AppType
    {
        Application = 0,
        MultiApplication,
        MultiApplicationExtral,
        MultiApplicationStrictMode,
        MultiApplicationStrictModeAfter
    }

    public enum ActivityCustom
    {
        AC_None = 0,
        AC_Unity,
        AC_Firebase
    }

    public enum App_Open_ad_Orien
    {
        Orien_Portraid = 0,
        Orien_Landscape
    }

    public enum MediaSourceType
    {
        googleadwords_int = 0,
        mintegral_int,
        applovin_int,
        Facebook_Ads,
        unityads_int,
        af_cross_promotion,
        ironsource_int,
        bytedanceglobal_int,
        restricted,
        Apple_Search_Ads,
        UnKnown
    }

    public class SDKManager : MonoBehaviour
    {
        public static event Action CBFinishloadDing = null;
        public Action<bool> CBPauseGame { get; set; }
#if UNITY_EDITOR
        public AppType typeApplication = AppType.MultiApplication;
        public ActivityCustom AcCustom = ActivityCustom.AC_None;
        public App_Open_ad_Orien OpenAdOrientation;
#endif
        [SerializeField] private bool isShowSplashLoading;
        public string AdFlyState = "0;0.14.5.2";
#if UNITY_ANDROID
        public string BigoAdsAndroi = "1;6;2.9.0.0";
#else
        public string BigoAdsiOS = "1;6;2.2.0";
#endif
        public bool isAdCanvasSandbox = false;
        public bool isDisableLog = false;

        public bool isAddandroidgms = false;
        public Transform canvasUpdate;
        public Transform CanvasLoading;
        public Transform PopupShowFirstAds;

        private SplashLoadingCtr splashLoadingCtr;
        private PopupNoConnectionCtr PopupLoseConnection;
        private PopupRateCtl ratePopControl;

        private GameObject PopupNotAllowGame;
        private GameObject PopupNotSupportIAP;

        private TakeScreenShootCallback _cb;

        public UpdateGameController updateGameController { get; set; }

        public static SDKManager Instance { get; private set; }

        public List<MoreGameOb> listMoreGame = new List<MoreGameOb>();
        float tcheckFirstShow = 0;
        bool isAdsShowFirsted = false;
        float tpreCheck = -1;
        public int flagTimeScale { get; set; }
        public bool flagNewDay { get; private set; }
        public bool isAllowShowFirstOpen { get; set; }
        static bool isLogRetenrion = false;
        int isShowWaitShowfull = 0;
        int stateShowFirstAds = 0;
        float timeCount4ShowFirstAds = 0;
        public bool deviceIsRooted { get; set; }
        [HideInInspector] public int totalTimePlayGame = 0;
        [HideInInspector] public int counSessionGame = 0;

        [HideInInspector] public MediaSourceType mediaType = MediaSourceType.UnKnown;
        [HideInInspector] public string mediaCampain = "";

        public Font fontReplace;
        public float fontSize = 1;
        public float lineSpacing = 1;

        public int timeWhenGetOnline { get; set; }
        public int timeOnline { get; set; }//minus

        private int statusCheckFollowPlay = 0;
        private float duarationPlayLv = 0;

        private void Awake()
        {
#if !UNITY_EDITOR
            if (isDisableLog)
            {
                Debug.unityLogger.filterLogType = LogType.Exception;
                Debug.developerConsoleVisible = false;
            }
            else
            {
                Debug.unityLogger.logEnabled = true;
                Debug.unityLogger.filterLogType = LogType.Log;
                Debug.developerConsoleVisible = true;
            }
#else
            Debug.unityLogger.logEnabled = true;
            Debug.unityLogger.filterLogType = LogType.Log;
            Debug.developerConsoleVisible = true;
#endif
            if (Instance == null)
            {
                Instance = this;
                counSessionGame = PlayerPrefs.GetInt("mem_gl_count_play", 0);
                counSessionGame++;
                PlayerPrefs.SetInt("mem_gl_count_play", counSessionGame);
                if (counSessionGame == 1)
                {
                    PlayerPrefs.SetInt("mem_ver_app_ins", AppConfig.verapp);
                    Debug.Log("mysdk: ver app install=" + PlayerPrefs.GetInt("mem_ver_app_ins", -1));
                }
                else
                {
                    Debug.Log("mysdk: ver app install=" + PlayerPrefs.GetInt("mem_ver_app_ins", -1));
                }
                mediaType = (MediaSourceType)PlayerPrefsBase.Instance().getInt("mem_mediatype", (int)MediaSourceType.UnKnown);
                mediaCampain = PlayerPrefsBase.Instance().getString("mem_mediacampain", "");
                totalTimePlayGame = PlayerPrefsBase.Instance().getInt("mem_tot_tim_gpla", 0);
                GameRes.transLevelOld2New();
                updateGameController = null;
                flagTimeScale = 0;
                CBPauseGame = null;
                isShowWaitShowfull = 0;
                stateShowFirstAds = 0;
                isAllowShowFirstOpen = true;
                deviceIsRooted = false;
                isAdsShowFirsted = true;
                timeWhenGetOnline = -1;
                timeOnline = -1;
                flagNewDay = isNewDay();
                statusCheckFollowPlay = PlayerPrefs.GetInt("mem_status_play", 0);

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (this != Instance) Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Log($"mysdk: sdkmanager Start deviceUniqueIdentifier={SystemInfo.deviceUniqueIdentifier}");
            //GameHelper.ScreenInfo();
            if (AdsHelper.isRemoveAds == 1)
            {
                Debug.Log("mysdk: sdkmanager removeads not show first");
                tcheckFirstShow = 10000;
            }
            else
            {
                int ss = AdsHelper.Instance.isShowOpenAds(true);
                if (ss == 0 || ss == 1)
                {
                    Debug.Log($"mysdk: sdkmanager isshowopen={ss}");
                    tcheckFirstShow = 10000;
                }
            }
            if (isShowSplashLoading)
            {
                // if (isShowSplash())
                {
                    var prefab = Resources.Load<GameObject>("Popup/PopupSplashLoading");
                    var ob = Instantiate(prefab, Vector3.zero, Quaternion.identity, CanvasLoading);
                    splashLoadingCtr = ob.GetComponent<SplashLoadingCtr>();
#if UNITY_IOS || UNITY_IPHONE
                    splashLoadingCtr.showLoading(3);
#else
                    splashLoadingCtr.showLoading(3);
#endif
                }
            }
            else
            {
                onSplashFinishLoding();
            }
            StartCoroutine(wait4loginLog());
            loadMoreGame();
            downMoreGame();

            if (flagNewDay && !isLogRetenrion)
            {
                Debug.Log("mysdk: Adjust Helper log retention");
                isLogRetenrion = true;
                AdjustHelper.LogEvent(AdjustEventName.Retention, null);
            }

            StartCoroutine(initNotify());

#if UNITY_EDITOR
            Debug.Log("mysdk: path game=" + Application.persistentDataPath);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR && CHECK_4INAPP
            if (isAdCanvasSandbox)
            {
                mygame.plugin.Android.GameHelperAndroid.printSigning();
            }
            bool isInstall = mygame.plugin.Android.GameHelperAndroid.isInstallFromGooglePlay();
            int rsesss = PlayerPrefsBase.Instance().getInt("mem_procinva_gema", 3);
            if (rsesss != 1 && rsesss != 2 && rsesss != 3 && rsesss != 101 && rsesss != 102 && rsesss != 103 && rsesss != 1985)
            {
                rsesss = 103;
            }
            if (!isInstall)
            {
                PlayerPrefsBase.Instance().setInt("mem_kt_cdtgpl", 1);
                if (rsesss == 101 || rsesss == 103)
                {
                    showNotAllowGame();
                    FIRhelper.logEvent($"game_invalid1_notallow");
                }
                FIRhelper.logEvent($"game_invalid1");
            }
            else
            {
                PlayerPrefsBase.Instance().setInt("mem_kt_cdtgpl", 0);
            }
            if (isInstall || (!isInstall && rsesss != 101 && rsesss != 103))
            {
                Debug.Log($"mysdk: sdkmanager bira 1");
                int flagccc = PlayerPrefsBase.Instance().getInt("mem_flag_check_bira", 1);
                mygame.plugin.Android.GameHelperAndroid.checkPiraCheck(flagccc);
            }
#endif
            if (statusCheckFollowPlay == 1)
            {
                onEndGame(Myapi.LevelPassStatus.EndOther, "terminate");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log($"mysdk: sdkmanager OnApplicationPause={pauseStatus}");
            closeWaitShowFull();
        }

        private IEnumerator initNotify()
        {
            yield return new WaitForSeconds(10);
            int memvernoti = PlayerPrefs.GetInt("mem_ver_data_noti", 0);
#if UNITY_ANDROID && !UNITY_EDITOR
            mygame.plugin.Android.GameHelperAndroid.setupEnviromentNotify();
            TextAsset txtnoti = Resources.Load<TextAsset>("LocalNotify/Android/dataNotify");
            if (txtnoti != null)
            {
                NotifyObject dataNoti = JsonUtility.FromJson<NotifyObject>(txtnoti.text);
                if (dataNoti.ver > memvernoti)
                {
                    PlayerPrefs.SetInt("mem_ver_data_noti", dataNoti.ver);
                    if (dataNoti.data.Count > 0)
                    {
                        mygame.plugin.Android.GameHelperAndroid.setupLocalNotifyNotify(dataNoti.data2String());
                    }
                }
            }
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            TextAsset txtnoti = Resources.Load<TextAsset>("LocalNotify/iOS/dataNotify");
            if (txtnoti != null)
            {
                NotifyObject dataNoti = JsonUtility.FromJson<NotifyObject>(txtnoti.text);
                if (dataNoti.ver > memvernoti)
                {
                    PlayerPrefs.SetInt("mem_ver_data_noti", dataNoti.ver);
                    GameHelperIos.clearAllNoti();
                    if (dataNoti.data.Count > 0)
                    {
                        foreach (NotiData item in dataNoti.data)
                        {
                            bool isrepeat = false;
                            for (int j = 0; j < item.repeat.Length; j++)
                            {
                                if (item.repeat[j] == '1')
                                {
                                    isrepeat = true;
                                    GameHelperIos.localNotify(item.titleNoti, item.msgNoti, item.hour, item.minus, j + 1);
                                }
                            }
                            if (!isrepeat)
                            {
                                GameHelperIos.localNotify(item.titleNoti, item.msgNoti, item.hour, item.minus, -1);
                            }
                        }
                    }
                }
            }
#endif
        }

        private void OnLowMemory()
        {
            FIRhelper.logEvent("app_low_mem");
            if (PlayerPrefs.GetInt("is_unload_when_lowmem", 0) == 1)
            {
                Resources.UnloadUnusedAssets();
            }
        }
        public bool isDeviceTest()
        {
#if UNITY_ANDROID
            string[] ldt = { "2c125e8002112db06bfa5e087059f3fd"
            ,"c2331d0694479013d1fcc5574b52af1f"
            ,"656ff28dd6e9d017a61a889b9887b063"
             };
#else//ios
            string[] ldt = { "F1D48B06-6ED7-452F-A7C1-4B85C4DBE4A4" };
#endif
            for (int i = 0; i < ldt.Length; i++)
            {
                if (ldt[i] != null && ldt[i].CompareTo(SystemInfo.deviceUniqueIdentifier) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isShowSplash()
        {
            int co = PlayerPrefs.GetInt("cf_open_ad_showat", -1);
            if (co == 1)
            {
                return true;
            }
            if (AdsHelper.isRemoveAds == 1 || (AdsHelper.Instance != null && AdsHelper.Instance.isShowOpenAds(true) > 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void onSplashFinishLoding()
        {
            if (CBFinishloadDing != null)
            {
                CBFinishloadDing();
            }
#if UNITY_IOS || UNITY_IPHONE
            AdsHelper.Instance.initAds();
            if (GameHelper.checkLvXaDu())
            {
                doShowFirst();
            }
#else
            doShowFirst();
#endif
            if (AdsHelper.Instance != null)
            {
                if (AdsHelper.Instance.currConfig.OpenAdIsShowFirstOpen != 2)
                {
                    isAllowShowFirstOpen = false;
                }
            }
        }

        public void onPlayGame(int lv, Myapi.LevelPassStatus statusplay, string where = "", string mode = "", Myapi.TypeDif type = Myapi.TypeDif.Normal)
        {
            duarationPlayLv = 0;
            if (statusCheckFollowPlay != 0)
            {
                Debug.LogError("mysdk: Follow Play game is wrong, let double check end game!!!!!!!!!!!!");
            }
            statusCheckFollowPlay = 1;
            PlayerPrefs.SetInt("mem_status_play", 1);
            PlayerPrefs.SetInt("mem_lv_play_4_log", lv);
            PlayerPrefs.SetString("mem_mode_play_4_log", mode);
            PlayerPrefs.SetInt("mem_dif_play_4_log", ((int)type));
            string sm = "";
            if (mode != null && mode.Length > 0)
            {
                sm = mode + "_";
            }
            FIRhelper.logEvent($"level_{sm}{lv:000}_play");
            if (statusplay == Myapi.LevelPassStatus.PlayAgain)
            {
                FIRhelper.logEvent($"level_{sm}{lv:000}_playagain");
            }
            else if (statusplay == Myapi.LevelPassStatus.PlayRe)
            {
                FIRhelper.logEvent($"level_{sm}{lv:000}_playre");
            }
            Myapi.LogEventApi.Instance().logLevel(lv, mode, type, statusplay, 0, where);
        }

        public void onEndGame(Myapi.LevelPassStatus stateEnd, string where = "")
        {
            if (statusCheckFollowPlay != 1)
            {
                Debug.LogError("mysdk: Follow Play game is wrong, let double check play game!!!!!!!!!!!!");
            }
            int lv = PlayerPrefs.GetInt("mem_lv_play_4_log", 1);
            string mode = PlayerPrefs.GetString("mem_mode_play_4_log", "");
            int idif = PlayerPrefs.GetInt("mem_dif_play_4_log", 1);
            Myapi.TypeDif type = (Myapi.TypeDif)idif;
            PlayerPrefs.SetInt("mem_status_play", 0);
            statusCheckFollowPlay = 0;
            totalTimePlayGame += (int)duarationPlayLv;
            PlayerPrefsBase.Instance().setInt("mem_tot_tim_gpla", totalTimePlayGame);
            string send = "";
            string subSen = "";
            if (stateEnd == Myapi.LevelPassStatus.Win || stateEnd == Myapi.LevelPassStatus.WinAgain)
            {
                send = "win";
                if (stateEnd == Myapi.LevelPassStatus.WinAgain)
                {
                    subSen = "winagain";
                }
            }
            else if (stateEnd == Myapi.LevelPassStatus.Lose || stateEnd == Myapi.LevelPassStatus.LoseAgain)
            {
                send = "lose";
                if (stateEnd == Myapi.LevelPassStatus.LoseAgain)
                {
                    subSen = "loseagain";
                }
            }
            else if (stateEnd == Myapi.LevelPassStatus.Skip)
            {
                send = "skip";
            }
            else
            {
                send = "endother";
            }
            string sm = "";
            if (mode != null && mode.Length > 0)
            {
                sm = mode + "_";
            }
            FIRhelper.logEvent($"level_{sm}{lv:000}_{send}");
            if (subSen.Length > 3)
            {
                FIRhelper.logEvent($"level_{sm}{lv:000}_{subSen}");
            }
            Myapi.LogEventApi.Instance().logLevel(lv, mode, type, stateEnd, (int)duarationPlayLv, where);
            if (duarationPlayLv > 10)
            {
                AdsHelper.Instance.checkChangeSpecialCon();
            }
        }

        public void changeFrameRate()
        {
            // #if UNITY_IOS || UNITY_IPHONE
            //             if (UnityEngine.iOS.Device.generation.ToString().Contains("iPhone"))
            //             {
            //                 int something = (int)UnityEngine.iOS.Device.generation;
            //                 if (something > 1 && something <= 32)
            //                 {
            //                     QualitySettings.vSyncCount = 0;
            //                     Application.targetFrameRate = 60;
            //                     Debug.Log("mysdk: ios low set pfs=" + Application.targetFrameRate);
            //                 }
            //                 else
            //                 {
            //                     Application.targetFrameRate = PlayerPrefs.GetInt("", 60);
            //                     QualitySettings.vSyncCount = 0;
            //                     Debug.Log("mysdk: ios high set pfs=" + Application.targetFrameRate);
            //                 }
            //             }
            //             else
            //             {
            //                 Application.targetFrameRate = PlayerPrefs.GetInt("", 60);
            //                 QualitySettings.vSyncCount = 0;
            //                 Debug.Log("mysdk: ios not iphone set pfs=" + Application.targetFrameRate);
            //             }
            // #else
            //             Application.targetFrameRate = PlayerPrefs.GetInt("", 60);
            //             QualitySettings.vSyncCount = 0;
            //             Debug.Log("mysdk: Android set pfs=" + Application.targetFrameRate);
            // #endif
        }

        IEnumerator wait4loginLog()
        {
            yield return new WaitForSeconds(0.05f);
            changeFrameRate();
            yield return new WaitForSeconds(2);
            Myapi.LogEventApi.Instance().logLogin();
            yield return new WaitForSeconds(5);
            Myapi.LogEventApi.Instance().pushMemLog();
        }

        public bool checkConnection(string title = "", string msg = "")
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                int countopenapp = PlayerPrefs.GetInt("mem_gl_count_play", 0);
                int cfCounOpenApp4CheckCon = PlayerPrefs.GetInt("cf_count_open_checkcon", 10);
                int cfLvcheckCon = PlayerPrefs.GetInt("cf_lv_checkcon", 50);
                if (countopenapp >= cfCounOpenApp4CheckCon || GameRes.LevelCommon() >= cfLvcheckCon)
                {
                    showPopupNotConnection(title, msg);
                    return false;
                }
            }
            return true;
        }

        public void showPopupNotConnection(string title = "", string msg = "")
        {
            if (PopupLoseConnection == null)
            {
                var prefab = Resources.Load<PopupNoConnectionCtr>("Popup/PopupNotConnection");
                PopupLoseConnection = Instantiate(prefab, Vector3.zero, Quaternion.identity, PopupShowFirstAds.transform.parent);
            }
            if (SdkUtil.isiPad())
            {
                PopupLoseConnection.transform.GetChild(0).localScale = Vector3.one * .9f;
            }
            PopupLoseConnection.gameObject.SetActive(true);
            PopupLoseConnection.show(title, msg);
        }

        public bool showRate()
        {
            Debug.Log("aaaaa showRate0");
            if (PlayerPrefs.GetInt("is_show_rate", 0) == 1)
            {
                return false;
            }
            Debug.Log("aaaaa showRate1");
#if UNITY_IOS || UNITY_IPHONE
            GameHelper.Instance.rate();
#else
            if (ratePopControl == null)
            {
                var prefab = Resources.Load<PopupRateCtl>("Popup/PopupRate");
                ratePopControl = Instantiate(prefab, Vector3.zero, Quaternion.identity, PopupShowFirstAds.transform.parent);
            }
            if (SdkUtil.isiPad())
            {
                ratePopControl.transform.GetChild(0).localScale = Vector3.one * .9f;
            }
            ratePopControl.gameObject.SetActive(true);
            RectTransform rc = ratePopControl.GetComponent<RectTransform>();
            rc.anchorMin = new Vector2(0, 0);
            rc.anchorMax = new Vector2(1, 1);
            rc.sizeDelta = new Vector2(0, 0);
            rc.anchoredPosition = Vector2.zero;
            rc.anchoredPosition3D = Vector3.zero;
#endif
            return true;
        }

        public bool isShowRate(int level)
        {
            string memlv = PlayerPrefs.GetString("mem_lv_showrate", ",6,10,30,35,40,");
            if (memlv.Length > 0)
            {
                if (!memlv.StartsWith(","))
                {
                    memlv = "," + memlv;
                }
                if (!memlv.EndsWith(","))
                {
                    memlv += ",";
                }
                if (memlv.Contains($",{level},"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isNewDay()
        {
            var today = DateTime.Today;
            var md = PlayerPrefs.GetInt("mem_day_4new", -1);
            var mm = PlayerPrefs.GetInt("mem_month_4new", -1);
            var my = PlayerPrefs.GetInt("mem_year_4new", -1);

            bool isnew = false;
            if (my < today.Year)
            {
                isnew = true;
            }
            else if (my == today.Year)
            {
                if (mm < today.Month)
                {
                    isnew = true;
                }
                else if (mm == today.Month)
                {
                    if (md < today.Day)
                    {
                        isnew = true;
                    }
                }
            }
            if (isnew)
            {
                Debug.Log("mysdk: SdkManager new day");
                PlayerPrefs.SetInt("mem_day_4new", today.Day);
                PlayerPrefs.SetInt("mem_month_4new", today.Month);
                PlayerPrefs.SetInt("mem_year_4new", today.Year);
            }
            return isnew;
        }

        private void Update()
        {
            if (statusCheckFollowPlay == 1)
            {
                if (Time.timeScale >= 0.00001f)
                {
                    duarationPlayLv += Time.unscaledDeltaTime;
                }
            }
            if (tcheckFirstShow < AdsHelper.Instance.currConfig.OpenAdTimeWaitShowFirst)
            {
                if (Time.timeScale <= 0.00001f)
                {
                    tcheckFirstShow += Time.unscaledDeltaTime;
                }
                else
                {
                    tcheckFirstShow += Time.deltaTime;
                }

                if (AdsHelper.Instance != null && !AdsHelper.Instance.isShowFulled)
                {
                    if (tcheckFirstShow - tpreCheck >= 0.5f || tcheckFirstShow >= AdsHelper.Instance.currConfig.OpenAdTimeWaitShowFirst)
                    {
                        tpreCheck = tcheckFirstShow;
                        if (isAllowShowFirstOpen)
                        {
                            if (AdsHelper.Instance.currConfig.OpenAdShowtype == 0
                                || AdsHelper.Instance.currConfig.OpenAdShowtype == 2
                                || AdsHelper.Instance.currConfig.OpenAdShowtype == 4
                            )
                            {
                                if (GameHelper.Instance.isOpenAdLoaded())
                                {
                                    tcheckFirstShow = 10000;
                                    doShowFirst();
                                }
                                else
                                {
                                    if (tcheckFirstShow >= AdsHelper.Instance.currConfig.OpenAdTimeWaitShowFirst)
                                    {
                                        if (AdsHelper.Instance.currConfig.OpenAdShowtype == 2)
                                        {
                                            if (!AdsHelper.Instance.isShowFulled)
                                            {
                                                stateShowFirstAds = 0;
                                                doShowFirst();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (AdsHelper.Instance.currConfig.OpenAdShowtype == 2 && tcheckFirstShow >= 12)
                                        {
                                            if (AdsHelper.Instance.isFull4Show(true, true, true))
                                            {
                                                tcheckFirstShow = 10000;
                                                doShowFirst();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (AdsHelper.Instance.isFull4Show(true, true, true))
                                {
                                    tcheckFirstShow = 10000;
                                    stateShowFirstAds = 1;
                                    PopupShowFirstAds.gameObject.SetActive(true);
                                    Time.timeScale = 0;
                                    timeCount4ShowFirstAds = 0;
                                    Debug.Log("mysdk: sdkmanager wait show first ok");
                                }
                                else
                                {
                                    if (tcheckFirstShow >= AdsHelper.Instance.currConfig.OpenAdTimeWaitShowFirst)
                                    {
                                        if (AdsHelper.Instance.currConfig.OpenAdShowtype == 3)
                                        {
                                            tcheckFirstShow = 10000;
                                            doShowFirst();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (AdsHelper.Instance != null && AdsHelper.Instance.isShowFulled)
                {
                    tcheckFirstShow = 10000;
                }
            }
            if (stateShowFirstAds > 0)
            {
                if (stateShowFirstAds == 1)
                {
                    timeCount4ShowFirstAds += Time.unscaledDeltaTime;
                    if (timeCount4ShowFirstAds >= 1.0f)
                    {
                        stateShowFirstAds = 0;
                        doShowFirst();
                    }
                }
            }
            if (flagTimeScale > 0 && Time.timeScale == 0)
            {
                flagTimeScale++;
                if (Input.GetMouseButton(0) && flagTimeScale > 200)
                {
                    SdkUtil.logd($"ads SDKManager flagTimeScale resumegame");
                    flagTimeScale = 0;
                    Time.timeScale = 1;
                    PopupShowFirstAds.gameObject.SetActive(false);
                }
            }

            if (isShowWaitShowfull > 0)
            {
                isShowWaitShowfull--;
                if (isShowWaitShowfull <= 0)
                {
                    PopupShowFirstAds.gameObject.SetActive(false);
                }
            }
        }

        public bool doShowFirst(bool isSetAllowShowFirst = false, bool isAllow = false)
        {
            Debug.Log($"mysdk: doShowFirst isSetAllowShowFirst={isSetAllowShowFirst}, isAllow={isAllow}, isAdsShowFirsted={isAdsShowFirsted}");
            if (!isAdsShowFirsted)
            {
                return false;
            }
            if (AdsHelper.Instance != null && !AdsHelper.Instance.isShowFulled)
            {
                if (isSetAllowShowFirst)
                {
                    isAllowShowFirstOpen = isAllow;
                }
                bool re = GameHelper.Instance.showAppOpenAdFirst();
                if (re)
                {
                    Debug.Log($"mysdk: doShowFirst show");
                    tcheckFirstShow = 10000;
                    flagTimeScale = 1;
                    Time.timeScale = 0;
                    if (CBPauseGame != null)
                    {
                        CBPauseGame.Invoke(true);
                    }
                    return true;
                }
                else
                {
                    Debug.Log($"mysdk: doShowFirst not show");
                    PopupShowFirstAds.gameObject.SetActive(false);
                    flagTimeScale = 0;
                    Time.timeScale = 1;
                    return false;
                }
            }
            else
            {
                PopupShowFirstAds.gameObject.SetActive(false);
                Time.timeScale = 1;
                return false;
            }
        }

        public void onFirstAdShow()
        {
            tcheckFirstShow = 10000;
            isAdsShowFirsted = false;
        }

        public void showWait4ShowFull()
        {
            isShowWaitShowfull = 90;
            PopupShowFirstAds.gameObject.SetActive(true);
            if (PopupShowFirstAds.transform.GetChild(1) != null)
            {
                PopupShowFirstAds.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        public void showWaitCommon()
        {
            PopupShowFirstAds.gameObject.SetActive(true);
            if (PopupShowFirstAds.transform.GetChild(1) != null)
            {
                PopupShowFirstAds.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        public void closeWaitShowFull()
        {
            if (isShowWaitShowfull > 0)
            {
                isShowWaitShowfull = 0;
                PopupShowFirstAds.gameObject.SetActive(false);
            }
        }

        public void Screenshot(TakeScreenShootCallback cb)
        {
            Debug.Log("mysdk: Sdkmanager Screenshot");
            _cb = cb;
            StartCoroutine(Take_a_screenshot());
        }

        private IEnumerator Take_a_screenshot()
        {
            yield return new WaitForEndOfFrame();
            var snap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            snap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            snap.Apply();
            _cb?.Invoke(snap);
        }

        public void showUpdate(int verup, int status, string gameid, string link = "", string title = "", string des = "")
        {
            if (updateGameController == null)
            {
                var prefab = Resources.Load<UpdateGameController>("Popup/PopupUpdate");
                updateGameController = Instantiate(prefab, Vector3.zero, Quaternion.identity, canvasUpdate);
            }
            updateGameController.showUpdate(verup, status, gameid, link, title, des);
        }

        public void saveMoreGame()
        {
            Debug.Log("mysdk: Sdkmanager saveMoreGame");
            string datamoregame = "";
            for (int i = 0; i < listMoreGame.Count; i++)
            {
                datamoregame += listMoreGame[i].ToString();
                if (i < (listMoreGame.Count - 1))
                {
                    datamoregame += ";";
                }
            }
            PlayerPrefs.SetString("my_more_game_data_mem", datamoregame);
        }

        public void loadMoreGame()
        {
            Debug.Log("mysdk: Sdkmanager loadMoreGame");
            string datamoregame = PlayerPrefs.GetString("my_more_game_data_mem", "");
            string[] slist = datamoregame.Split(new char[] { ';' });
            foreach (string sgame in slist)
            {
                MoreGameOb more = new MoreGameOb();
                if (more.fromString(sgame))
                {
                    listMoreGame.Add(more);
                }
            }
        }

        public void downMoreGame()
        {
            Debug.Log("mysdk: Sdkmanager downMoreGame");
            string btmore = PlayerPrefs.GetString("my_more_game_btmore_mem", "");
            if (btmore.Length > 10)
            {
                ImageLoader.loadImageTexture(btmore);
            }
            foreach (var game in listMoreGame)
            {
                if (game.isStatic == 1)
                {
                    DownLoadUtil.downloadImg(game.playAbleAds, "");
                }
                else
                {
                    if (game.playOnClient == 1)
                    {
                        DownLoadUtil.downloadText(game.playAbleAds, "");
                    }
                }
                ImageLoader.loadImageTexture(game.icon);
            }
        }

        //==========================================================================================
        public void showNotAllowGame()
        {
            if (PopupNotAllowGame == null)
            {
                var prefab = Resources.Load<GameObject>("Popup/PopupNotAllowGame");
                PopupNotAllowGame = Instantiate(prefab, Vector3.zero, Quaternion.identity, PopupShowFirstAds.transform.parent);
            }
            PopupNotAllowGame.SetActive(true);
            RectTransform rc = PopupNotAllowGame.GetComponent<RectTransform>();
            rc.anchorMin = new Vector2(0, 0);
            rc.anchorMax = new Vector2(1, 1);
            rc.sizeDelta = new Vector2(0, 0);
            rc.anchoredPosition = Vector2.zero;
            rc.anchoredPosition3D = Vector3.zero;
        }

        public void showNotSupportIAP()
        {
            if (PopupNotSupportIAP == null)
            {
                var prefab = Resources.Load<GameObject>("Popup/PopupNotSupportIAP");
                PopupNotSupportIAP = Instantiate(prefab, Vector3.zero, Quaternion.identity, PopupShowFirstAds.transform.parent);
            }
            PopupNotSupportIAP.SetActive(true);
            RectTransform rc = PopupNotSupportIAP.GetComponent<RectTransform>();
            rc.anchorMin = new Vector2(0, 0);
            rc.anchorMax = new Vector2(1, 1);
            rc.sizeDelta = new Vector2(0, 0);
            rc.anchoredPosition = Vector2.zero;
            rc.anchoredPosition3D = Vector3.zero;
        }
        //==========================================================================================
    }
}