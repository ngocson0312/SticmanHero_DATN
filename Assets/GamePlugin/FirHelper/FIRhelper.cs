//#define FIRBASE_ENABLE
//#define FIR_DATABASE_E
#define ENABLE_LOG_ADMOB_REVENUE
#define ENABLE_FIRHELPER_LOG_CLIENT

//#define ENABLE_GETCONFIG
//#define ENABLE_GETFCM_TOKEN

//#define ENABLE_CHECKAND

#define ENABLE_LOG_UNITY
//#define ENABLE_LOG_FB

using System;
using System.Collections;
using System.Collections.Generic;
using MyJson;
using UnityEngine;
using UnityEngine.Analytics;
#if FIRBASE_ENABLE
using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;
using Firebase.RemoteConfig;

#if FIR_DATABASE_E
using Firebase.Database;

#endif

#elif ENABLE_GETCONFIG
using Firebase.RemoteConfig;
#endif

namespace mygame.sdk
{
    public class FIRhelper : MonoBehaviour
    {
        public static FIRhelper Instance { get; private set; }
        public static event Action CBGetconfig = null;

        public int idxOther { get; private set; }

        public int statusInitFir = 0;
        private static int isFetchConfig = 0;

        private bool statelogData = false;
        public string playerName { get; set; }
        public string gcm_id { get; set; }
        public int countRewardAd4heart;
        private PromoGameOb gamePromoCurr = null;
        private static List<QueueLogFir> listWaitLog = new List<QueueLogFir>();

        public static float ValueDailyAdRevenew = 0.003f;
        public static long AdmobRevenewDivide = 1000000000;
        public static int isLogAdmobRevenueAppsFlyer = 1;

#if FIRBASE_ENABLE
        private static List<System.Action<Firebase.DependencyStatus>> initializedMethods =
            new List<System.Action<Firebase.DependencyStatus>>();

        private static Firebase.DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
#endif

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                gcm_id = PlayerPrefs.GetString("get_gcm_id", "");
                isLogAdmobRevenueAppsFlyer = PlayerPrefs.GetInt("is_log_admob_va2af", 1);
                AdmobRevenewDivide = PlayerPrefs.GetInt("ad_va_divide", 1000000000);

#if ENABLE_LOG_UNITY
                Dictionary<string, object> dicparam = new Dictionary<string, object>();
                dicparam.Add("Login", 1);
                Analytics.CustomEvent("Login", dicparam);
#endif
                checkOvertimePromoClick();
                listWaitLog.Clear();
            }
        }

        // Use this for initialization 
        private void Start()
        {
            Debug.Log("mysdk: fir Start 0");
            Invoke("checkand", 30);
            Invoke("checkCoty", 30);
            checkUpdate();

#if FIRBASE_ENABLE
            statusInitFir = 0;
            isFetchConfig = 0;
            dependencyStatus = DependencyStatus.UnavailableOther;
            if (dependencyStatus != DependencyStatus.Available)
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    dependencyStatus = task.Result;
                    if (dependencyStatus == DependencyStatus.Available)
                    {
                        var app = FirebaseApp.DefaultInstance;
                        //Debug.Log("mysdk: Firebase is ready.");
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            InitializeFirebase();
                        });
                    }
                    else
                    {
                        Debug.Log("mysdk: Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
#endif
        }

        private void checkUpdate()
        {
            int ver = PlayerPrefs.GetInt("update_ver", AppConfig.verapp);
            int ss = PlayerPrefs.GetInt("update_status", 0);
            int cfday = PlayerPrefs.GetInt("update_day_cf", 0);
            string gameid = PlayerPrefs.GetString("update_gameid", AppConfig.appid);
            string link = PlayerPrefs.GetString("update_link", "");
            string title = PlayerPrefs.GetString("update_title", "");
            string des = PlayerPrefs.GetString("update_des", "");
            int memverShow = PlayerPrefs.GetInt("update_ver_show", 0);
            int dayInstallGame = PlayerPrefs.GetInt("update_day_install", -1);
            int curday = DateTime.Now.Year * 365 + DateTime.Now.DayOfYear;
            int countday = 0;
            if (dayInstallGame < 0)
            {
                PlayerPrefs.SetInt("update_day_install", curday);
            }
            else
            {
                countday = curday - dayInstallGame;
            }
            if (ver >= AppConfig.verapp && countday >= cfday)
            {
                if (ss == 1)
                {
                    long tcurr = GameHelper.CurrentTimeMilisReal() / 60000;
                    int tmem = PlayerPrefs.GetInt("update_memtime_show", 0);
                    if (memverShow >= AppConfig.verapp && (tcurr - tmem) <= 1800)
                    {
                        ss = 0;
                    }
                }
                if (ss != 0 && (gameid.Length > 3 || link.Length > 10))
                {
                    SDKManager.Instance.showUpdate(ver, ss, gameid, link, title, des);
                    if (ss == 1)
                    {
                        PlayerPrefs.SetInt("update_status", 0);
                        PlayerPrefs.SetInt("update_ver_show", AppConfig.verapp);
                        long tshow = GameHelper.CurrentTimeMilisReal() / 60000;
                        PlayerPrefs.SetInt("update_memtime_show", (int)tshow);
                    }
                }
            }
        }

        private void checkand()
        {
#if UNITY_ANDROID && ENABLE_CHECKAND && !UNITY_EDITOR
                        /*
                         * for android
                         * fill package name to check my app
                        */
                        string pkgCheckAndroid = AppConfig.appid;
                        string pkg = Application.identifier;
                        if ((pkg == null || pkgCheckAndroid == null) || (string.CompareOrdinal(pkg, pkgCheckAndroid) != 0)) 
                        {
                            AdsHelper.Instance = null;
                            double aaaaa = 0.0f;
                            double aaaaa1 = 1234560.12334564764574f;
                            double aaaaa2 = 1234560.12334564764574f;
                            while (true) {
                                for (int i = 0; i < 1234567890; i++)
                                {
                                    aaaaa = aaaaa1 * aaaaa2;
                                    aaaaa = Math.Sqrt(aaaaa);
                                    if(i >= 1234567) {
                                        i = 0;
                                    }
                                }
                            }
                        }
#endif
        }

        public void checkcounty()
        {
            Invoke("checkCoty", 30);
        }

        public void checkCoty()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (GameHelper.Instance != null && GameHelper.Instance.countryCode != null && GameHelper.Instance.countryCode.CompareTo("cn") == 0)
            {
                AdsHelper.Instance = null;
                double aaaaa = 0.0f;
                double aaaaa1 = 1234560.12334564764574f;
                double aaaaa2 = 1234560.12334564764574f;
                while (true)
                {
                    for (int i = 0; i < 1234567890; i++)
                    {
                        aaaaa = aaaaa1 * aaaaa2;
                        aaaaa = Math.Sqrt(aaaaa);
                        if (i >= 1234567)
                        {
                            i = 0;
                        }
                    }
                }
            }
#endif
        }

        public void OnDestroy()
        {
            if (Instance != null && Instance == this)
            {
#if FIRBASE_ENABLE
                Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
                Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
#endif
            }
        }

        private void callUpdateGcm()
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                StartCoroutine(waitUpdateGcm());
            });
        }

        IEnumerator waitUpdateGcm()
        {
            yield return new WaitForSeconds(5);
            //Myapi.SingleTonApi<Myapi.UpdateGcmApi, Myapi.ApiResultOb>.Instance.update();
        }

        public void InitializeFirebase()
        {
            Debug.Log("mysdk: firHelper InitializeFirebase");
#if FIRBASE_ENABLE
            statusInitFir = 1;
#if FIR_DATABASE_E
#if UNITY_ANDROID
            myApp.SetEditorDatabaseUrl("https://run-clash-3d.firebaseio.com/");
#elif UNITY_IOS || UNITY_IPHONE
            myApp.SetEditorDatabaseUrl("https://run-clash-3d.firebaseio.com");
#endif
#endif
            //message
            Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    SdkUtil.logd("Fetch canceled.");
                }
                else if (task.IsFaulted)
                {
                    SdkUtil.logd("Fetch encountered an error.");
                }
                else if (task.IsCompleted)
                {
                    SdkUtil.logd("Fetch completed successfully!");
                }

                var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
                switch (info.LastFetchStatus)
                {
                    case Firebase.RemoteConfig.LastFetchStatus.Success:
                        //vvvFirebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateFetched();
                        Debug.Log(String.Format("mysdk: fir Remote data loaded and ready (last fetch time {0}-{1}).",
                            SdkUtil.toTimestamp(info.FetchTime), SdkUtil.CurrentTimeMilis()));
                        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(taskac =>
                        {
                            parserConfig();
                        });
                        break;
                    case Firebase.RemoteConfig.LastFetchStatus.Failure:
                        switch (info.LastFetchFailureReason)
                        {
                            case Firebase.RemoteConfig.FetchFailureReason.Error:
                                SdkUtil.logd("Fetch failed for unknown reason");
                                break;
                            case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                                SdkUtil.logd("Fetch throttled until " + info.ThrottledEndTime);
                                break;
                        }

                        break;
                    case Firebase.RemoteConfig.LastFetchStatus.Pending:
                        SdkUtil.logd("Latest Fetch call still pending.");
                        break;
                }
            });

            FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
            foreach (QueueLogFir memlog in listWaitLog)
            {
                if (memlog != null)
                {
                    if (memlog.memdic != null && memlog.memdic.Count > 0)
                    {
                        Parameter[] prams = new Parameter[memlog.memdic.Count];
                        int idx = 0;
                        foreach (var item in memlog.memdic)
                        {
                            prams[idx] = new Parameter(item.Key, item.Value.ToString());
                            idx++;
                        }
                        Firebase.Analytics.FirebaseAnalytics.LogEvent(memlog.nameevent, prams);
#if ENABLE_FIRHELPER_LOG_CLIENT
                        Debug.Log("mysdk: fir log21 event=" + memlog.nameevent);
#endif
                    }
                    else
                    {
                        FirebaseAnalytics.LogEvent(memlog.nameevent);
#if ENABLE_FIRHELPER_LOG_CLIENT
                        Debug.Log("mysdk: fir log11 event=" + memlog.nameevent);
#endif
                    }
                }
            }
            listWaitLog.Clear();
#endif
        }

#if FIRBASE_ENABLE || ENABLE_GETFCM_TOKEN
        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("mysdk: Token: " + token.Token);
                if (token != null && token.Token != null && token.Token.Length > 0)
                {
                    gcm_id = token.Token;
                    PlayerPrefs.SetString("get_gcm_id", gcm_id);
                    if (PlayerPrefs.GetInt("mem_send_gcm_ok", 0) != 1)
                    {
                        //callUpdateGcm();
                    }
                }
            });
        }
#endif
#if FIRBASE_ENABLE

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SdkUtil.logd("Receied a new message");
                if (e.Message.From.Length > 0)
                    SdkUtil.logd("from: " + e.Message.RawData);
                if (e.Message.Data.Count > 0)
                {
                    SdkUtil.logd("data:");
                    foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                        e.Message.Data)
                    {
                        SdkUtil.logd(iter.Key + ": " + iter.Value);
#if UNITY_IOS || UNITY_IPHONE
                        if (iter.Key.Equals("gift_box"))
                        {
                            PlayerPrefs.SetString("mem_gift_box_ios", iter.Value);
                        }
#endif
                    }
                }
            });
        }
#endif

        public static void setUserProperty(string name, string property)
        {
#if FIRBASE_ENABLE
            if (Instance != null && Instance.statusInitFir == 1)
            {
                Firebase.Analytics.FirebaseAnalytics.SetUserProperty(name, property);
            }
#endif
        }
        public static void logEvent(string title)
        {
#if FIRBASE_ENABLE
            if (Instance != null && Instance.statusInitFir == 1)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent(title);
#if ENABLE_FIRHELPER_LOG_CLIENT
                Debug.Log("mysdk: fir log1 event=" + title);
#endif
            }
            else
            {
                QueueLogFir mem = new QueueLogFir();
                mem.nameevent = title;
                listWaitLog.Add(mem);
                Debug.Log("mysdk: fir memlog=" + title);
            }
#endif

#if ENABLE_LOG_FB
            FBHelper.Instance.logEvent(title);
#endif

#if ENABLE_LOG_UNITY
            Analytics.CustomEvent(title);
#endif
        }
        public static void logEvent(string name, Dictionary<string, object> dicParams)
        {
#if FIRBASE_ENABLE
            if (Instance != null && Instance.statusInitFir == 1)
            {
                if (dicParams != null && dicParams.Count > 0)
                {
                    Parameter[] prams = new Parameter[dicParams.Count];
                    int idx = 0;
                    foreach (var item in dicParams)
                    {
                        prams[idx] = new Parameter(item.Key, item.Value.ToString());
                        idx++;
                    }
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(name, prams);
                }
                else
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
                }
#if ENABLE_FIRHELPER_LOG_CLIENT
                Debug.Log("mysdk: fir log2 event=" + name);
#endif
            }
            else
            {
                QueueLogFir mem = new QueueLogFir();
                mem.nameevent = name;
                mem.memdic = dicParams;
                listWaitLog.Add(mem);
                Debug.Log("mysdk: fir memlog=" + name);
            }
#endif

#if ENABLE_LOG_FB
            FBHelper.Instance.logEvent(name, parameter, value);
#endif

#if ENABLE_LOG_UNITY
            Analytics.CustomEvent(name, dicParams);
#endif
        }

        //int typeAds 0-bn, 1-full, 2-gift
        public static void logEventAdsPaidAdmob(int typeAds, string adunitId, int precisionType, string currencyCode, long rvalueMicros)
        {
            int vapost = PlayerPrefs.GetInt("mem_va_of_ad_postfir", AppConfig.PerValuePostFir);
            long newvalueMicros = rvalueMicros * vapost / 100;
#if ENABLE_FIRHELPER_LOG_CLIENT
            Debug.Log($"mysdk: firhelper logEventAdsPaidAdmob typeAds={typeAds}, adunitId={adunitId}, precisionType={precisionType}, currencyCode={currencyCode} valueMicros={rvalueMicros} newva={newvalueMicros}");
#endif

#if ENABLE_AppsFlyer
            if (isLogAdmobRevenueAppsFlyer == 1)
            {
                Dictionary<string, string> additionalParameters = new Dictionary<string, string>();
                string adformat = "";
                if (typeAds == 0)
                {
                    adformat = "banner";
                }
                else if (typeAds == 1)
                {
                    adformat = "interstitial";
                }
                else if (typeAds == 2)
                {
                    adformat = "reward_video";
                }
                else if (typeAds == 3)
                {
                    adformat = "app_open_ads";
                }
                additionalParameters.Add(AFAdRevenueEvent.AD_UNIT, adunitId);
                additionalParameters.Add(AFAdRevenueEvent.AD_TYPE, adformat);
                double dv = newvalueMicros / ((double)AdmobRevenewDivide);
                AppsFlyerSDK.AppsFlyerAdRevenue.logAdRevenue("googleadmob", AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob, dv, currencyCode, additionalParameters);
            }
#endif

#if FIRBASE_ENABLE && ENABLE_LOG_ADMOB_REVENUE
            Firebase.Analytics.Parameter[] paramspaid = new Firebase.Analytics.Parameter[5];
            paramspaid[0] = new Firebase.Analytics.Parameter("valuemicros", newvalueMicros);
            paramspaid[1] = new Firebase.Analytics.Parameter("currency", currencyCode);
            paramspaid[2] = new Firebase.Analytics.Parameter("precision", precisionType);
            paramspaid[3] = new Firebase.Analytics.Parameter("adunitid", adunitId);
            paramspaid[4] = new Firebase.Analytics.Parameter("network", "admob");

            double maxLtv = PlayerPrefs.GetFloat("mem_paid_max_ltv", 0);
            double admobLtv = PlayerPrefs.GetFloat("mem_paid_admob_ltv", 0);
            double ironLtv = PlayerPrefs.GetFloat("mem_paid_iron_ltv", 0);
            double ltv = maxLtv + ironLtv + (admobLtv + newvalueMicros) / ((double)AdmobRevenewDivide);
            if (ltv >= ValueDailyAdRevenew)
            {
                PlayerPrefs.SetFloat("mem_paid_max_ltv", 0);
                PlayerPrefs.SetFloat("mem_paid_admob_ltv", 0);
                PlayerPrefs.SetFloat("mem_paid_iron_ltv", 0);
            }
            else
            {
                PlayerPrefs.SetFloat("mem_paid_admob_ltv", (int)(admobLtv + newvalueMicros));
            }
            logEventAdsPaid(paramspaid, ltv);
#endif
        }
        public static void logEventAdsPaidMax(string adunitId, string adUnitIdentifier, string adSource, string adformat, double rrevenue, string countrycode, string netplacement)
        {
            float vapost = PlayerPrefs.GetInt("mem_va_of_ad_postfir", AppConfig.PerValuePostFir);
            double newrevenue = rrevenue * vapost / 100.0f;
#if ENABLE_FIRHELPER_LOG_CLIENT
            Debug.Log($"mysdk: firhelper logEventAdsPaidMax adunitId={adunitId}, adUnitIdentifier={adUnitIdentifier}, adSource={adSource}, adformat={adformat}, revenue={rrevenue}, newva={newrevenue}, countrycode={countrycode}, netplacement={netplacement}");
#endif

#if ENABLE_AppsFlyer
            Dictionary<string, string> additionalParameters = new Dictionary<string, string>();
            additionalParameters.Add(AFAdRevenueEvent.AD_UNIT, adunitId);
            additionalParameters.Add(AFAdRevenueEvent.AD_TYPE, adformat);
            additionalParameters.Add(AFAdRevenueEvent.COUNTRY, countrycode);
            additionalParameters.Add(AFAdRevenueEvent.PLACEMENT, netplacement);
            string lownet = adSource.ToLower();
            AppsFlyerSDK.AppsFlyerAdRevenue.logAdRevenue(lownet, AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax, newrevenue, "USD", additionalParameters);
#endif

#if FIRBASE_ENABLE
            Firebase.Analytics.Parameter[] paramspaid = new Firebase.Analytics.Parameter[6];
            paramspaid[0] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdPlatform, "AppLovin");
            paramspaid[1] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdSource, adSource);
            paramspaid[2] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdUnitName, adUnitIdentifier);
            paramspaid[3] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdFormat, adformat);
            paramspaid[4] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterValue, newrevenue);
            paramspaid[5] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterCurrency, "USD");
            double maxLtv = PlayerPrefs.GetFloat("mem_paid_max_ltv", 0);
            double admobLtv = PlayerPrefs.GetFloat("mem_paid_admob_ltv", 0);
            double ironLtv = PlayerPrefs.GetFloat("mem_paid_iron_ltv", 0);
            double ltv = (maxLtv + newrevenue) + ironLtv + admobLtv / ((double)AdmobRevenewDivide);
            if (ltv >= ValueDailyAdRevenew)
            {
                PlayerPrefs.SetFloat("mem_paid_max_ltv", 0);
                PlayerPrefs.SetFloat("mem_paid_admob_ltv", 0);
                PlayerPrefs.SetFloat("mem_paid_iron_ltv", 0);
            }
            else
            {
                PlayerPrefs.SetFloat("mem_paid_max_ltv", (float)(maxLtv + newrevenue));
            }
            logEventAdsPaid(paramspaid, ltv);
#endif
        }
        public static void logEventAdsPaidIron(string adunitId, string adSource, string adunitName, string adformat, double rrevenue, string countrycode, string netplacement)
        {
            float vapost = PlayerPrefs.GetInt("mem_va_of_ad_postfir", AppConfig.PerValuePostFir);
            double newrevenue = rrevenue * vapost / 100.0f;
#if ENABLE_FIRHELPER_LOG_CLIENT
            Debug.Log($"mysdk: firhelper logEventAdsPaidIron adunitId={adunitId}, adSource={adSource}, adunitName={adunitName}, adformat={adformat}, revenue={rrevenue}, newva={newrevenue}, countrycode={countrycode}, netplacement={netplacement}");
#endif

#if ENABLE_AppsFlyer
            Dictionary<string, string> additionalParameters = new Dictionary<string, string>();
            additionalParameters.Add(AFAdRevenueEvent.AD_UNIT, adunitId);
            additionalParameters.Add(AFAdRevenueEvent.AD_TYPE, adunitName);
            additionalParameters.Add(AFAdRevenueEvent.COUNTRY, countrycode);
            additionalParameters.Add(AFAdRevenueEvent.PLACEMENT, netplacement);
            string lownet = adSource.ToLower();
            AppsFlyerSDK.AppsFlyerAdRevenue.logAdRevenue(lownet, AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource, newrevenue, "USD", additionalParameters);
#endif

#if FIRBASE_ENABLE
            Firebase.Analytics.Parameter[] paramspaid = new Firebase.Analytics.Parameter[6];
            paramspaid[0] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdPlatform, "ironSource");
            paramspaid[1] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdSource, adSource);
            paramspaid[2] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdUnitName, adunitName);
            paramspaid[3] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterAdFormat, adformat);
            paramspaid[4] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterValue, newrevenue);
            paramspaid[5] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterCurrency, "USD");
            double maxLtv = PlayerPrefs.GetFloat("mem_paid_max_ltv", 0);
            double admobLtv = PlayerPrefs.GetFloat("mem_paid_admob_ltv", 0);
            double ironLtv = PlayerPrefs.GetFloat("mem_paid_iron_ltv", 0);
            double ltv = (ironLtv + newrevenue) + maxLtv + admobLtv / ((double)AdmobRevenewDivide);
            if (ltv >= ValueDailyAdRevenew)
            {
                PlayerPrefs.SetFloat("mem_paid_max_ltv", 0);
                PlayerPrefs.SetFloat("mem_paid_admob_ltv", 0);
                PlayerPrefs.SetFloat("mem_paid_iron_ltv", 0);
            }
            else
            {
                PlayerPrefs.SetFloat("mem_paid_iron_ltv", (float)(ironLtv + newrevenue));
            }
            logEventAdsPaid(paramspaid, ltv);
#endif
        }

#if FIRBASE_ENABLE
        static void logEventAdsPaid(Firebase.Analytics.Parameter[] paramspaid, double ltv)
        {
            if (paramspaid != null && paramspaid.Length > 0)
            {
                FirebaseAnalytics.LogEvent("ad_impression", paramspaid);
            }
            if (ltv >= ValueDailyAdRevenew)
            {
#if ENABLE_FIRHELPER_LOG_CLIENT
                Debug.Log($"mysdk: firhelper logEventAdsPaidDaily ltv={ltv}");
#endif
                logEventTroasAdrevenue(ltv);
            }
        }

        static void logEventTroasAdrevenue(double ltv)
        {
            Firebase.Analytics.Parameter[] paramspaid = new Firebase.Analytics.Parameter[2];
            paramspaid[0] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterValue, ltv);
            paramspaid[1] = new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterCurrency, "USD");
            FirebaseAnalytics.LogEvent("Daily_Ads_Revenue", paramspaid);
        }
#endif
        public void parserConfig()
        {
            try
            {
                Debug.Log("mysdk: parserConfig0");
                isFetchConfig = 1;
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
#if UNITY_ANDROID
                ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue("ads_region_android");
#else
                ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue("ads_region_ios");
#endif
                bool isConfigAds = false;
                ObjectAdsCf cgdf = null;
                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    var listads = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                    if (listads != null || listads.Count > 0)
                    {
                        AdsHelper.Instance.adsConfigAllRegion.Clear();
                        foreach (KeyValuePair<string, object> item in listads)
                        {
                            IDictionary<string, object> oitm = (IDictionary<string, object>)item.Value;
                            string[] arrkey = item.Key.Split(new char[] { ',' });
                            foreach (string ctc in arrkey)
                            {
                                Debug.Log("mysdk: parserConfig ads country=" + ctc);
                                ObjectAdsCf obads = new ObjectAdsCf(ctc);
                                if (cgdf != null)
                                {
                                    obads.coppyFromOther(cgdf);
                                }
                                else
                                {
                                    obads.loadFromPlayerPrefs();
                                }
                                if (oitm.ContainsKey("maskAdsStatus"))
                                {
                                    obads.maskAdsStatus = Convert.ToInt32(oitm["maskAdsStatus"]);
                                }
                                if (oitm.ContainsKey("fullTotalOfday"))
                                {
                                    obads.fullTotalOfday = Convert.ToInt32(oitm["fullTotalOfday"]);
                                }
                                if (oitm.ContainsKey("fullLevelStart"))
                                {
                                    obads.fullLevelStart = Convert.ToInt32(oitm["fullLevelStart"]);
                                }
                                if (oitm.ContainsKey("fullSessionStart"))
                                {
                                    obads.fullSessionStart = Convert.ToInt32(oitm["fullSessionStart"]);
                                }
                                if (oitm.ContainsKey("fullTimeStart"))
                                {
                                    obads.fullTimeStart = Convert.ToInt32(oitm["fullTimeStart"]) * 1000;
                                }
                                if (oitm.ContainsKey("defaultFullNumover"))
                                {
                                    obads.fullDefaultNumover = Convert.ToInt32(oitm["defaultFullNumover"]);
                                }
                                if (oitm.ContainsKey("fullLoadAdsMobStatic"))
                                {
                                    obads.fullLoadAdsMobStatic = Convert.ToInt32(oitm["fullLoadAdsMobStatic"]);
                                }
                                if (oitm.ContainsKey("fullShowPlaying"))
                                {
                                    obads.fullShowPlaying = Convert.ToInt32(oitm["fullShowPlaying"]);
                                }
                                if (oitm.ContainsKey("fullExcluseRun"))
                                {
                                    obads.excluseFullrunning = (string)oitm["fullExcluseRun"];
                                    obads.parSerExcluseFull(obads.excluseFullrunning);
                                }
                                if (oitm.ContainsKey("fullDeltatime"))
                                {
                                    obads.fullDeltatime = 1000 * Convert.ToInt32(oitm["fullDeltatime"]);
                                }
                                if (oitm.ContainsKey("loadMaxLow"))
                                {
                                    obads.isLoadMaxLow = Convert.ToInt32(oitm["loadMaxLow"]);
                                }
                                if (oitm.ContainsKey("giftTotalOfday"))
                                {
                                    obads.giftTotalOfday = Convert.ToInt32(oitm["giftTotalOfday"]);
                                }
                                if (oitm.ContainsKey("giftDeltatime"))
                                {
                                    obads.giftDeltatime = 1000 * Convert.ToInt32(oitm["giftDeltatime"]);
                                }
                                if (oitm.ContainsKey("Sdk_version_show_banner"))
                                {
                                    obads.verShowBanner = Convert.ToInt32(oitm["Sdk_version_show_banner"]);
                                }
                                if (oitm.ContainsKey("interval_showfull"))
                                {
                                    obads.intervalnumoverfull = (string)oitm["interval_showfull"];
                                    obads.parserIntervalnumoverfull(obads.intervalnumoverfull);
                                }

                                if (oitm.ContainsKey("bn_change_static"))
                                {
                                    obads.isChangeBNStatic = Convert.ToInt32(oitm["bn_change_static"]);
                                }
                                if (oitm.ContainsKey("step_show_banner"))
                                {
                                    obads.stepShowBanner = (string)oitm["step_show_banner"];
                                    obads.parSerStepBN(obads.stepShowBanner);
                                }

                                if (oitm.ContainsKey("step_show_native"))
                                {
                                    string step = (string)oitm["step_show_native"];
                                    obads.parSerStepNative(step);
                                }

                                if (oitm.ContainsKey("step_show_full"))
                                {
                                    obads.stepShowFull = (string)oitm["step_show_full"];
                                    obads.parSerStepFull(obads.stepShowFull);
                                }

                                if (oitm.ContainsKey("step_show_gift"))
                                {
                                    obads.stepShowGift = (string)oitm["step_show_gift"];
                                    obads.parSerStepGift(obads.stepShowGift);
                                }

                                if (oitm.ContainsKey("cf_state_show_applovin"))
                                {
                                    obads.stateShowAppLlovin = Convert.ToInt32(oitm["cf_state_show_applovin"]);
                                }

                                if (oitm.ContainsKey("lv_show_apploovin"))
                                {
                                    obads.levelShowAppLovin = Convert.ToInt32(oitm["lv_show_apploovin"]);
                                }

                                if (oitm.ContainsKey("cf_try_reload_applovin"))
                                {
                                    obads.tryloadApplovin = Convert.ToInt32(oitm["cf_try_reload_applovin"]);
                                }

                                if (oitm.ContainsKey("cf_load_other_max_loaded"))
                                {
                                    obads.loadOtherWhenLoadedApplovinButNotReady = Convert.ToInt32(oitm["cf_load_other_max_loaded"]);
                                }

                                if (oitm.ContainsKey("special_con"))
                                {
                                    IDictionary<string, object> dicSpec = (IDictionary<string, object>)oitm["special_con"];
                                    obads.parSerSpecial(dicSpec);
                                }

                                //openads
                                if (oitm.ContainsKey("cf_open_ad_typen"))
                                {
                                    obads.OpenAdShowtype = Convert.ToInt32(oitm["cf_open_ad_typen"]);
                                }
                                if (oitm.ContainsKey("cf_open_ad_showat"))
                                {
                                    obads.OpenAdShowat = Convert.ToInt32(oitm["cf_open_ad_showat"]);
                                }
                                if (oitm.ContainsKey("cf_open_ad_show_firstopen"))
                                {
                                    obads.OpenAdIsShowFirstOpen = Convert.ToInt32(oitm["cf_open_ad_show_firstopen"]);
                                }
                                if (oitm.ContainsKey("cf_open_ad_deltime"))
                                {
                                    obads.OpenAdDelTimeOpen = Convert.ToInt32(oitm["cf_open_ad_deltime"]);
                                }
                                if (oitm.ContainsKey("cf_open_ad_level_show"))
                                {
                                    obads.OpenadLvshow = Convert.ToInt32(oitm["cf_open_ad_level_show"]);
                                }
                                if (oitm.ContainsKey("cf_open_ad_wait_first"))
                                {
                                    obads.OpenAdTimeWaitShowFirst = Convert.ToInt32(oitm["cf_open_ad_wait_first"]);
                                }
                                if (oitm.ContainsKey("cf_type_myopenad"))
                                {
                                    obads.typeMyopenAd = Convert.ToInt32(oitm["cf_type_myopenad"]);
                                }

                                if (oitm.ContainsKey("cf_lv_static_ads"))
                                {
                                    obads.cfLvStaticAds = Convert.ToInt32(oitm["cf_lv_static_ads"]);
                                }

                                if (oitm.ContainsKey("cf_lv_ktro"))
                                {
                                    int lvban = Convert.ToInt32(oitm["cf_lv_ktro"]);
                                    PlayerPrefs.SetInt("cf_lv_ktro", lvban);
                                }

                                if (ctc.CompareTo("default") == 0)
                                {
                                    if (oitm.ContainsKey("is_log_admob_va2af"))
                                    {
                                        isLogAdmobRevenueAppsFlyer = Convert.ToInt32(oitm["is_log_admob_va2af"]);
                                        PlayerPrefs.SetInt("is_log_admob_va2af", isLogAdmobRevenueAppsFlyer);
                                    }

                                    if (oitm.ContainsKey("ad_va_divide"))
                                    {
                                        int tmpva = Convert.ToInt32(oitm["ad_va_divide"]);
                                        PlayerPrefs.SetInt("ad_va_divide", tmpva);
                                        AdmobRevenewDivide = tmpva;
                                    }
                                }

                                AdsHelper.Instance.adsConfigAllRegion.Add(ctc, obads);
                                if (cgdf == null)
                                {
                                    if (AdsHelper.Instance.adsConfigAllRegion.ContainsKey(GameHelper.CountryDefault))
                                    {
                                        cgdf = AdsHelper.Instance.adsConfigAllRegion[GameHelper.CountryDefault];
                                    }
                                }
                            }
                        }
                        isConfigAds = true;
                    }
                }

#if UNITY_ANDROID
                v = FirebaseRemoteConfig.DefaultInstance.GetValue("ads_region2_android");
#else
                v = FirebaseRemoteConfig.DefaultInstance.GetValue("ads_region2_ios");
#endif
                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    var listads = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                    if (listads != null || listads.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> item in listads)
                        {
                            IDictionary<string, object> oitm = (IDictionary<string, object>)item.Value;
                            string[] arrkey = item.Key.Split(new char[] { ',' });
                            foreach (string ctc in arrkey)
                            {
                                Debug.Log("mysdk: parserfull2 country=" + ctc);
                                ObjectAdsCf obads;
                                if (!AdsHelper.Instance.adsConfigAllRegion.ContainsKey(ctc))
                                {
                                    obads = new ObjectAdsCf(ctc);
                                    if (cgdf != null)
                                    {
                                        obads.coppyFromOther(cgdf);
                                    }
                                    else
                                    {
                                        obads.loadFromPlayerPrefs();
                                    }
                                }
                                else
                                {
                                    obads = AdsHelper.Instance.adsConfigAllRegion[ctc];
                                }
                                
                                if (oitm.ContainsKey("full2LevelStart"))
                                {
                                    obads.full2LevelStart = Convert.ToInt32(oitm["full2LevelStart"]);
                                }
                                if (oitm.ContainsKey("full2Deltatime"))
                                {
                                    obads.full2Deltatime = 1000 * Convert.ToInt32(oitm["full2Deltatime"]);
                                }
                                if (oitm.ContainsKey("full2Numover"))
                                {
                                    obads.full2Numover = Convert.ToInt32(oitm["full2Numover"]);
                                }
                                if (oitm.ContainsKey("step_show_full2"))
                                {
                                    obads.stepShowFull2 = (string)oitm["step_show_full2"];
                                    obads.parSerStepFull2(obads.stepShowFull2);
                                }

                                
                                if (cgdf == null)
                                {
                                    if (AdsHelper.Instance.adsConfigAllRegion.ContainsKey(GameHelper.CountryDefault))
                                    {
                                        cgdf = AdsHelper.Instance.adsConfigAllRegion[GameHelper.CountryDefault];
                                    }
                                }
                            }
                        }
                        isConfigAds = true;
                    }
                }

#if UNITY_ANDROID
                v = FirebaseRemoteConfig.DefaultInstance.GetValue("admob_defaultid_android");
#else
                v = FirebaseRemoteConfig.DefaultInstance.GetValue("admob_defaultid_ios");
#endif

                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    IDictionary<string, object> cfdfid = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                    foreach (KeyValuePair<string, object> adsnet in cfdfid)
                    {
                        if (adsnet.Key.Equals("admob"))
                        {
                            IDictionary<string, object> cfadsType = (IDictionary<string, object>)adsnet.Value;
                            foreach (KeyValuePair<string, object> adstype in cfadsType)
                            {
                                if (adstype.Key.Contains("cf_banner"))
                                {
                                    AdsHelper.Instance.adsAdmobMy.setBannerId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_open"))
                                {
                                    PlayerPrefs.SetString($"mem_df0_open_id", (string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_full_img"))
                                {
                                    AdsHelper.Instance.adsAdmobMy.setSplashId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_full"))
                                {
                                    AdsHelper.Instance.adsAdmobMy.setFullId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_gift"))
                                {
                                    AdsHelper.Instance.adsAdmobMy.setGiftId((string)adstype.Value);
                                }
                            }
                            isConfigAds = true;
                        }
                        else if (adsnet.Key.Equals("applovin"))
                        {
                            IDictionary<string, object> cfadsType = (IDictionary<string, object>)adsnet.Value;
                            foreach (KeyValuePair<string, object> adstype in cfadsType)
                            {
                                if (adstype.Key.Contains("cf_banner"))
                                {
                                    AdsHelper.Instance.adsApplovinMax.setBannerId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_open"))
                                {
                                    PlayerPrefs.SetString($"mem_df6_open_id", (string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_full_img"))
                                {
                                    AdsHelper.Instance.adsApplovinMax.setSplashId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_full"))
                                {
                                    AdsHelper.Instance.adsApplovinMax.setFullId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_gift"))
                                {
                                    AdsHelper.Instance.adsApplovinMax.setGiftId((string)adstype.Value);
                                }
                            }
                            isConfigAds = true;
                        }
                        else if (adsnet.Key.Equals("iron"))
                        {
                            IDictionary<string, object> cfadsType = (IDictionary<string, object>)adsnet.Value;
                            foreach (KeyValuePair<string, object> adstype in cfadsType)
                            {
                                if (adstype.Key.Contains("cf_banner"))
                                {
                                    AdsHelper.Instance.adsIron.setBannerId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_open"))
                                {
                                    PlayerPrefs.SetString($"mem_df3_open_id", (string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_full_img"))
                                {
                                    AdsHelper.Instance.adsIron.setSplashId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_full"))
                                {
                                    AdsHelper.Instance.adsIron.setFullId((string)adstype.Value);
                                }
                                else if (adstype.Key.Contains("cf_gift"))
                                {
                                    AdsHelper.Instance.adsIron.setGiftId((string)adstype.Value);
                                }
                            }
                            isConfigAds = true;
                        }
                    }
                }

#if UNITY_ANDROID
                v = FirebaseRemoteConfig.DefaultInstance.GetValue("admob_floor_cpm_android");
#else
                v = FirebaseRemoteConfig.DefaultInstance.GetValue("admob_floor_cpm_ios");
#endif
                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    var listfloorecpm = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                    if (listfloorecpm != null || listfloorecpm.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> item in listfloorecpm)
                        {
                            IDictionary<string, object> oitm = (IDictionary<string, object>)item.Value;
                            string[] arrkey = item.Key.Split(new char[] { ',' });
                            foreach (string ctc in arrkey)
                            {
                                Debug.Log("mysdk: parserConfig ecpm floor country=" + ctc);
                                ObjectAdsCf obads;
                                if (AdsHelper.Instance.adsConfigAllRegion.ContainsKey(ctc))
                                {
                                    obads = AdsHelper.Instance.adsConfigAllRegion[ctc];
                                }
                                else
                                {
                                    obads = new ObjectAdsCf(ctc);
                                    if (cgdf != null)
                                    {
                                        obads.coppyFromOther(cgdf);
                                    }
                                    else
                                    {
                                        obads.loadFromPlayerPrefs();
                                    }
                                    AdsHelper.Instance.adsConfigAllRegion.Add(ctc, obads);
                                }
                                if (oitm.ContainsKey("cf_banner"))
                                {
                                    obads.stepFloorECPMBanner = (string)oitm["cf_banner"];
                                }
                                if (oitm.ContainsKey("cf_full"))
                                {
                                    obads.stepFloorECPMFull = (string)oitm["cf_full"];
                                }
                                if (oitm.ContainsKey("cf_gift"))
                                {
                                    obads.stepFloorECPMGift = (string)oitm["cf_gift"];
                                }
                            }
                        }
                        isConfigAds = true;
                    }
                }
                if (isConfigAds)
                {
                    AdsHelper.Instance.configWithRegion();
                }
                parserPromo();
                parserUpdateGame();
                parserOpenAds();
                parserMoregame();
                parserCfGame();
                FIRParserOtherConfig.parserInGameConfig();
                if (CBGetconfig != null)
                {
                    CBGetconfig();
                }
#endif
                Debug.Log("mysdk: parserConfig1");
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ex=" + ex);
            }
        }
        public static string FirConfigGet(string key, string vdef)
        {
            if (isFetchConfig != 1)
            {
                return vdef;
            }
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            if (FirebaseRemoteConfig.DefaultInstance == null)
            {
                return vdef;
            }
            //Debug.LogError("mysdk: FirConfigGet string=" + key);
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                return v.StringValue;
            }
            else
            {
                return vdef;
            }
#else
            return vdef;
#endif
        }
        public static long FirConfigGet(string key, long vdef)
        {
            if (isFetchConfig != 1)
            {
                return vdef;
            }
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            if (FirebaseRemoteConfig.DefaultInstance == null)
            {
                return vdef;
            }
            //Debug.LogError("mysdk: FirConfigGet long=" + key);
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                return v.LongValue;
            }
            else
            {
                return vdef;
            }
#else
            return vdef;
#endif
        }
        public static double FirConfigGet(string key, double vdef)
        {
            if (isFetchConfig != 1)
            {
                return vdef;
            }
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            if (FirebaseRemoteConfig.DefaultInstance == null)
            {
                return vdef;
            }
            //Debug.LogError("mysdk: FirConfigGet double=" + key);
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                return v.DoubleValue;
            }
            else
            {
                return vdef;
            }
#else
            return vdef;
#endif
        }
        private void parserPromo()
        {
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            //my game promo
#if UNITY_ANDROID
            string key_cf_game_promo = "cf_game_promo_android";
#else
            string key_cf_game_promo = "cf_game_promo_ios";
#endif
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key_cf_game_promo);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                Debug.Log("mysdk: cf_game_promo");
                var obPromo = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                if (obPromo != null || obPromo.Count > 0)
                {
                    int verPromo = 0;
                    foreach (KeyValuePair<string, object> item in obPromo)
                    {
                        if (item.Key.Equals("ver"))
                        {
                            verPromo = Convert.ToInt32(item.Value);
                            break;
                        }
                    }
                    int memverpromo = PlayerPrefs.GetInt("mem_ver_gamepromo", 1);
                    if (verPromo > memverpromo)
                    {
                        gamePromoCurr = null;
                        PlayerPrefs.SetInt("mem_ver_gamepromo", verPromo);
                        PlayerPrefs.SetString("cf_game_promo", "");
                        PlayerPrefs.SetString("mem_game_will_promo", "");
                    }
                    string memlistgame = PlayerPrefs.GetString("cf_game_promo", "");
                    var obmemgames = (IDictionary<string, object>)JsonDecoder.DecodeText(memlistgame);
                    List<object> listmemgames = null;
                    if (obmemgames != null && obmemgames.ContainsKey("games"))
                    {
                        listmemgames = (List<object>)obmemgames["games"];
                    }

                    List<PromoGameOb> listnewgame = new List<PromoGameOb>();
                    bool isnewGame = false;
                    foreach (KeyValuePair<string, object> item in obPromo)
                    {
                        if (item.Key.Equals("games"))
                        {
                            var listGames = (List<object>)item.Value;
                            foreach (var itemgame in listGames)
                            {
                                var gamepromo = (IDictionary<string, object>)itemgame;
                                PromoGameOb g = new PromoGameOb();
                                g.name = (string)gamepromo["name"];
                                g.pkg = (string)gamepromo["pkg"];
                                g.appid = (string)gamepromo["appid"];
                                g.setImg((string)gamepromo["icon"]);
                                g.link = (string)gamepromo["link"];
                                g.des = (string)gamepromo["des"];
                                listnewgame.Add(g);
                                if (!isnewGame)
                                {
                                    if (listmemgames != null)
                                    {
                                        bool hasinlist = false;
                                        for (int kk = 0; kk < listmemgames.Count; kk++)
                                        {
                                            gamepromo = (IDictionary<string, object>)listmemgames[kk];
                                            string pkgmm = (string)gamepromo["pkg"];
                                            if (pkgmm.Equals(g.pkg))
                                            {
                                                hasinlist = true;
                                                break;
                                            }
                                        }
                                        if (!hasinlist)
                                        {
                                            isnewGame = true;
                                        }
                                    }
                                    else
                                    {
                                        isnewGame = true;
                                    }
                                }
                            }
                        }
                    }
                    if (isnewGame)
                    {
                        memlistgame = "{\"games\":[";
                        for (int kk = 0; kk < listnewgame.Count; kk++)
                        {
                            if (kk != 0)
                            {
                                memlistgame += ",{";
                            }
                            else
                            {
                                memlistgame += "{";
                            }
                            memlistgame += $"\"name\":\"{listnewgame[kk].name}\",";
                            memlistgame += $"\"pkg\":\"{listnewgame[kk].pkg}\",";
                            memlistgame += $"\"appid\":\"{listnewgame[kk].appid}\",";
                            memlistgame += $"\"icon\":\"{listnewgame[kk].getImgs()}\",";
                            memlistgame += $"\"link\":\"{listnewgame[kk].link}\",";
                            memlistgame += $"\"des\":\"{listnewgame[kk].des}\"";

                            memlistgame += "}";
                        }
                        memlistgame += "]}";
                        PlayerPrefs.SetString("cf_game_promo", memlistgame);
                        gamePromoCurr = null;
                        getGamePromo();
                    }
                }
            }
#endif
        }
        private void parserUpdateGame()
        {
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            //my game promo
#if UNITY_ANDROID
            string key_cf_game_update = "cf_game_update_android";
#else
            string key_cf_game_update = "cf_game_update_ios";
#endif
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key_cf_game_update);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                var obupdategame = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                if (obupdategame != null || obupdategame.Count > 0)
                {
                    int statusUpdate = Convert.ToInt32(obupdategame["status"]);
                    int verUpdate = Convert.ToInt32(obupdategame["version"]);
                    int daycf = 0;
                    if (obupdategame.ContainsKey("day"))
                    {
                        daycf = Convert.ToInt32(obupdategame["day"]);
                    }
                    string gameid = AppConfig.appid;
                    string titleup = "";
                    string desup = "";
                    string link = "";
                    if (obupdategame.ContainsKey("gameid"))
                    {
                        gameid = (string)obupdategame["gameid"];
                    }
                    if (obupdategame.ContainsKey("title"))
                    {
                        titleup = (string)obupdategame["title"];
                    }
                    if (obupdategame.ContainsKey("des"))
                    {
                        desup = (string)obupdategame["des"];
                    }
                    if (obupdategame.ContainsKey("link"))
                    {
                        link = (string)obupdategame["link"];
                    }
                    PlayerPrefs.SetInt("update_ver", verUpdate);
                    PlayerPrefs.SetInt("update_status", statusUpdate);
                    PlayerPrefs.GetInt("update_day_cf", daycf);
                    PlayerPrefs.SetString("update_gameid", gameid);
                    PlayerPrefs.SetString("update_link", link);
                    PlayerPrefs.SetString("update_title", titleup);
                    PlayerPrefs.SetString("update_des", desup);
                    checkUpdate();
                }
            }
#endif
        }
        private void parserOpenAds()
        {
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
#if UNITY_ANDROID
            string key_cf_game_update = "cf_my_open_ads_android";
#else
            string key_cf_game_update = "cf_my_open_ads_ios";
#endif
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key_cf_game_update);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                var obupdategame = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                if (MyAdsOpen.Instance != null && (obupdategame != null || obupdategame.Count > 0))
                {
                    int ver = Convert.ToInt32(obupdategame["ver"]);
                    int memver = PlayerPrefs.GetInt("mem_ver_myopen", 0);
                    Debug.Log("mysdk: parserOpenAds vercf=" + ver + ", memver=" + memver);
                    if (ver > memver)
                    {
                        if (obupdategame.ContainsKey("games"))
                        {
                            PlayerPrefs.SetString("mem_data_myopen_ad", v.StringValue);
                            PlayerPrefs.SetInt("mem_ver_myopen", ver);
                            MyAdsOpen.Instance.ListMyAdsOpen.Clear();
                            var games = (List<object>)obupdategame["games"];
                            foreach (var itemgame in games)
                            {
                                var gameopen = (IDictionary<string, object>)itemgame;
                                MyOpenAdsOb myads = new MyOpenAdsOb();
                                MyAdsOpen.Instance.ListMyAdsOpen.Add(myads);

                                myads.linkAds = (string)gameopen["linkAds"];
                                myads.gameId = (string)gameopen["gameId"];
                            }
                            AdsHelper.Instance.loadOpenAd();
                        }
                    }
                }
            }
#endif
        }
        private void parserMoregame()
        {
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
#if UNITY_ANDROID
            string key_cf_game_update = "cf_more_game_android";
#else
            string key_cf_game_update = "cf_more_game_ios";
#endif
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue(key_cf_game_update);
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                var obupdategame = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                if (obupdategame != null || obupdategame.Count > 0)
                {
                    int idob = Convert.ToInt32(obupdategame["id"]);
                    int memid = PlayerPrefs.GetInt("my_more_game_id_mem", 0);
                    Debug.Log("mysdk: parserMoregame idcf=" + idob + ", idmem=" + memid);
                    if (idob > memid)
                    {
                        SDKManager.Instance.listMoreGame.Clear();
                        if (obupdategame.ContainsKey("btMore"))
                        {
                            PlayerPrefs.SetString("my_more_game_btmore_mem", (string)obupdategame["btMore"]);
                        }
                        if (obupdategame.ContainsKey("games"))
                        {
                            var games = (List<object>)obupdategame["games"];
                            foreach (var itemgame in games)
                            {
                                var gamemore = (IDictionary<string, object>)itemgame;
                                MoreGameOb g = new MoreGameOb();
                                SDKManager.Instance.listMoreGame.Add(g);
                                g.gameName = (string)gamemore["name"];
                                g.icon = (string)gamemore["icon"];
                                g.gameId = (string)gamemore["gameId"];
                                g.playAbleAds = (string)gamemore["playAbleAds"];
                                g.isStatic = Convert.ToInt32(gamemore["isStatic"]);
                                g.linkStore = (string)gamemore["linkStore"];
                            }
                            SDKManager.Instance.saveMoreGame();
                            SDKManager.Instance.downMoreGame();
                        }
                        PlayerPrefs.SetInt("my_more_game_id_mem", idob);
                    }
                }
            }
#endif
        }
        private void parserCfGame()
        {
#if FIRBASE_ENABLE || ENABLE_GETCONFIG
            ConfigValue v = FirebaseRemoteConfig.DefaultInstance.GetValue("level_show_request_IDFA_iOS");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int lvshow = PlayerPrefs.GetInt("lv_show_request_idfa", 2);
                if (lvshow > 0)
                {
                    int per = (int)v.LongValue;
                    PlayerPrefs.SetInt("lv_show_request_idfa", per);
                }
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_ver_os_show_idfa");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_ver_os_show_idfa", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_ver_game_show_idfa");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_ver_game_show_idfa", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("LvShowRate");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefs.SetString("mem_lv_showrate", v.StringValue);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_count_open_checkcon");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_count_open_checkcon", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_lv_checkcon");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_lv_checkcon", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_trathai_xadu");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_trathai_xadu", per);
                Debug.Log($"mysdk: firhelper cf_trathai_xadu={per}");
            }

            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_adcanvas_CMP");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_adcanvas_CMP", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_adcanvas_enable");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_adcanvas_enable", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_adcanvas_alshow");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_adcanvas_alshow", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_adcanvas_click");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("cf_adcanvas_click", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_flag_mylog");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                int per = (int)v.LongValue;
                PlayerPrefs.SetInt("mem_flag_log", per);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_url_mylog");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefs.SetString("mem_url_log", v.StringValue);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("is_unload_when_lowmem");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefs.SetInt("is_unload_when_lowmem", (int)v.LongValue);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_game_kitr");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefsBase.Instance().setInt("mem_procinva_gema", (int)v.LongValue);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_game_flag_bira");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefsBase.Instance().setInt("mem_flag_check_bira", (int)v.LongValue);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("is_vali_appsf");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefsBase.Instance().setInt("is_vali_appsf", (int)v.LongValue);
            }
            v = FirebaseRemoteConfig.DefaultInstance.GetValue("cf_per_post_adva");
            if (v.StringValue != null && v.StringValue.Length > 0)
            {
                PlayerPrefs.SetInt("mem_va_of_ad_postfir", (int)v.LongValue);
            }


#if ENABLE_ADCANVAS
#if UNITY_IOS || UNITY_IPHONE
            string keyadcanvas = "adcanvas_df_v1_ios";
#else
            string keyadcanvas = "adcanvas_df_v1_android";
#endif
            if (AdCanvasHelper.Instance != null)
            {
                v = FirebaseRemoteConfig.DefaultInstance.GetValue(keyadcanvas);
                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    var adcanvasdefault = (IDictionary<string, object>)JsonDecoder.DecodeText(v.StringValue);
                    if (adcanvasdefault != null || adcanvasdefault.Count > 0)
                    {
                        if (adcanvasdefault.ContainsKey("6x5"))
                        {
                            List<object> listgameAdcanvas = (List<object>)adcanvasdefault["6x5"];
                            string dataadcanvasdf = "";
                            for (int kk = 0; kk < listgameAdcanvas.Count; kk++)
                            {
                                var adcanvasgame = (IDictionary<string, object>)listgameAdcanvas[kk];
                                string img = (string)adcanvasgame["img"];
                                string link = (string)adcanvasgame["link"];
                                if (dataadcanvasdf.Length > 0)
                                {
                                    dataadcanvasdf += "#";
                                }
                                dataadcanvasdf += img;
                                if (link.Length > 5)
                                {
                                    dataadcanvasdf += ";" + link;
                                }
                            }
                            Debug.Log($"mysdk: fir adcanvas get cf5=" + dataadcanvasdf);
                            PlayerPrefs.SetString("adcanvas_v1_6x5", dataadcanvasdf);
                        }
                        AdCanvasHelper.Instance.configDefault();
                    }
                }
#if UNITY_IOS || UNITY_IPHONE
                keyadcanvas = "adcanvas_cf_list_show_ios";
#else
                keyadcanvas = "adcanvas_cf_list_show_and";
#endif
                v = FirebaseRemoteConfig.DefaultInstance.GetValue(keyadcanvas);
                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    PlayerPrefs.SetString("cf_list_adcanvas", v.StringValue);
                    AdCanvasHelper.Instance.initListAdcanvas(v.StringValue);
                }
#if UNITY_IOS || UNITY_IPHONE
                keyadcanvas = "adcanvas_cf_listvideo_ios";
#else
                keyadcanvas = "adcanvas_cf_listvideo_and";
#endif
                v = FirebaseRemoteConfig.DefaultInstance.GetValue(keyadcanvas);
                if (v.StringValue != null && v.StringValue.Length > 0)
                {
                    PlayerPrefs.SetString("cf_adcanvas_video", v.StringValue);
                    AdCanvasHelper.Instance.initListAdcanvas(v.StringValue);
                }
            }
#endif//ENABLE_ADCANVAS

#endif
        }

        public PromoGameOb nextGamePromo()
        {
            gamePromoCurr = null;
            return getGamePromo();
        }
        private void downLoadIconPromoGame(List<object> listmemgames, string pkgstart)
        {
            if (listmemgames != null && listmemgames.Count > 0)
            {
                int idxDown = -1;
                if (pkgstart == null || pkgstart.Length == 0)
                {
                    idxDown = 0;
                }
                else
                {
                    for (int i = 0; i < listmemgames.Count; i++)
                    {
                        var gamepromo = (IDictionary<string, object>)listmemgames[i];
                        string pkgcuu = (string)gamepromo["pkg"];
                        if (pkgstart.CompareTo(pkgcuu) == 0)
                        {
                            idxDown = i;
                            break;
                        }
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    var gamepromo = (IDictionary<string, object>)listmemgames[idxDown];
                    idxDown++;
                    if (idxDown >= listmemgames.Count)
                    {
                        idxDown = 0;
                    }

                    string linkicon = (string)gamepromo["icon"];
                    string nameimg = ImageLoader.url2nameData(linkicon, 1);
                    if (!System.IO.File.Exists(DownLoadUtil.pathCache() + "/" + nameimg))
                    {
                        ImageLoader.loadImageTexture(linkicon, 100, 100, null);
                    }
                }
            }
        }
        public PromoGameOb getGameHasIcon()
        {
            string memlistgame = PlayerPrefs.GetString("cf_game_promo", "");
            string memclick = PlayerPrefs.GetString("mem_game_promo_click", "");
            if (memlistgame != null && memlistgame.Length > 10)
            {
                var obmemgames = (IDictionary<string, object>)JsonDecoder.DecodeText(memlistgame);
                List<object> listmemgames = null;
                if (obmemgames != null && obmemgames.ContainsKey("games"))
                {
                    listmemgames = (List<object>)obmemgames["games"];
                }
                if (listmemgames != null)
                {
                    for (int i = 0; i < listmemgames.Count; i++)
                    {
                        var gamepromo = (IDictionary<string, object>)listmemgames[i];
                        string pkgcuu = (string)gamepromo["pkg"];
                        if (!memclick.Contains(pkgcuu))
                        {
                            string linkicon = (string)gamepromo["icon"];
                            string nameimg = ImageLoader.url2nameData(linkicon, 1);
                            if (System.IO.File.Exists(DownLoadUtil.pathCache() + "/" + nameimg))
                            {
                                PromoGameOb tmpCurr = new PromoGameOb();
                                tmpCurr.pkg = pkgcuu;
                                tmpCurr.name = (string)gamepromo["name"];
                                tmpCurr.appid = (string)gamepromo["appid"];
                                tmpCurr.setImg((string)gamepromo["icon"]);
                                tmpCurr.link = (string)gamepromo["link"];
                                tmpCurr.des = (string)gamepromo["des"];
                                return tmpCurr;
                            }
                        }
                    }
                    for (int i = 0; i < listmemgames.Count; i++)
                    {
                        var gamepromo = (IDictionary<string, object>)listmemgames[i];
                        string pkgcuu = (string)gamepromo["pkg"];
                        string linkicon = (string)gamepromo["icon"];
                        string nameimg = ImageLoader.url2nameData(linkicon, 1);
                        if (System.IO.File.Exists(DownLoadUtil.pathCache() + "/" + nameimg))
                        {
                            PromoGameOb tmpCurr = new PromoGameOb();
                            tmpCurr.pkg = pkgcuu;
                            tmpCurr.name = (string)gamepromo["name"];
                            tmpCurr.appid = (string)gamepromo["appid"];
                            tmpCurr.setImg((string)gamepromo["icon"]);
                            tmpCurr.link = (string)gamepromo["link"];
                            tmpCurr.des = (string)gamepromo["des"];
                            return tmpCurr;
                        }
                    }
                }
            }
            return null;
        }
        public PromoGameOb getGamePromo()
        {
            if (gamePromoCurr != null)
            {
                return gamePromoCurr;
            }
            else
            {
                string memlistgame = PlayerPrefs.GetString("cf_game_promo", "");
                string memclick = PlayerPrefs.GetString("mem_game_promo_click", "");
                if (memlistgame != null && memlistgame.Length > 10)
                {
                    var obmemgames = (IDictionary<string, object>)JsonDecoder.DecodeText(memlistgame);
                    List<object> listmemgames = null;
                    if (obmemgames != null && obmemgames.ContainsKey("games"))
                    {
                        listmemgames = (List<object>)obmemgames["games"];
                    }
                    if (listmemgames != null)
                    {
                        string pkggameWillPromo = PlayerPrefs.GetString("mem_game_will_promo", "");
                        PromoGameOb tmpCurr = null;
                        for (int i = 0; i < listmemgames.Count; i++)
                        {
                            var gamepromo = (IDictionary<string, object>)listmemgames[i];
                            string pkgcuu = (string)gamepromo["pkg"];
                            if (!memclick.Contains(pkgcuu))
                            {
                                if (pkggameWillPromo == null || pkggameWillPromo.Length < 5)
                                {
                                    gamePromoCurr = new PromoGameOb();
                                    gamePromoCurr.pkg = pkgcuu;
                                    gamePromoCurr.name = (string)gamepromo["name"];
                                    gamePromoCurr.appid = (string)gamepromo["appid"];
                                    gamePromoCurr.setImg((string)gamepromo["icon"]);
                                    gamePromoCurr.link = (string)gamepromo["link"];
                                    gamePromoCurr.des = (string)gamepromo["des"];
                                }
                                else
                                {
                                    if (pkgcuu.Equals(pkggameWillPromo))
                                    {
                                        gamePromoCurr = new PromoGameOb();
                                        gamePromoCurr.pkg = pkgcuu;
                                        gamePromoCurr.name = (string)gamepromo["name"];
                                        gamePromoCurr.appid = (string)gamepromo["appid"];
                                        gamePromoCurr.setImg((string)gamepromo["icon"]);
                                        gamePromoCurr.link = (string)gamepromo["link"];
                                        gamePromoCurr.des = (string)gamepromo["des"];
                                    }
                                    else if (tmpCurr == null)
                                    {
                                        tmpCurr = new PromoGameOb();
                                        tmpCurr.pkg = pkgcuu;
                                        tmpCurr.name = (string)gamepromo["name"];
                                        tmpCurr.appid = (string)gamepromo["appid"];
                                        tmpCurr.setImg((string)gamepromo["icon"]);
                                        tmpCurr.link = (string)gamepromo["link"];
                                        tmpCurr.des = (string)gamepromo["des"];
                                    }
                                }
                                if (listmemgames.Count > 1 && gamePromoCurr != null)
                                {
                                    int idxnex = i;
                                    for (int pp = 0; pp < listmemgames.Count; pp++)
                                    {
                                        idxnex++;
                                        if (idxnex >= listmemgames.Count)
                                        {
                                            idxnex = 0;
                                        }
                                        gamepromo = (IDictionary<string, object>)listmemgames[idxnex];
                                        pkgcuu = (string)gamepromo["pkg"];
                                        if (!memclick.Contains(pkgcuu))
                                        {
                                            PlayerPrefs.SetString("mem_game_will_promo", pkgcuu);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        if (gamePromoCurr == null && tmpCurr != null)
                        {
                            gamePromoCurr = tmpCurr;
                        }
                        downLoadIconPromoGame(listmemgames, gamePromoCurr.pkg);
                    }
                }
                else
                {
                    gamePromoCurr = null;
                }
            }

            return gamePromoCurr;
        }
        public void onClickPromoGame(PromoGameOb game)
        {
            if (game != null)
            {
                string memclick = PlayerPrefs.GetString("mem_game_promo_click", "");
                memclick += (";" + game.pkg + "," + GameHelper.CurrentTimeMilisReal());
                PlayerPrefs.SetString("mem_game_promo_click", memclick);
                logEvent("click_promo");

#if ENABLE_AppsFlyer
                Dictionary<string, string> ParamPromo = new Dictionary<string, string>();
                ParamPromo.Add("af_promo_appid", AppConfig.appid);
                AppsFlyerSDK.AppsFlyer.attributeAndOpenStore(game.appid, "cross_promo_campaign", ParamPromo, mygame.sdk.AppsFlyerHelperScript.Instance);
#endif

                if (gamePromoCurr == null || (gamePromoCurr != null && game.name.CompareTo(gamePromoCurr.name) == 0))
                {
                    gamePromoCurr = null;
                    getGamePromo();
                }
                if (game.link != null && game.link.Length > 10)
                {
                    GameHelper.Instance.gotoLink(game.link);
                }
                else
                {
#if UNITY_IOS || UNITY_IPHONE
                    GameHelper.Instance.gotoStore(game.appid);
#else
                    GameHelper.Instance.gotoStore(game.pkg);
#endif
                }
            }
        }
        private void checkOvertimePromoClick()
        {
            string memclick = PlayerPrefs.GetString("mem_game_promo_click", "");
            string[] listmemclick = memclick.Split(new char[] { ';' });
            memclick = "";
            bool isovettime = false;
            for (int i = 0; i < listmemclick.Length; i++)
            {
                string[] gameclick = listmemclick[i].Split(new char[] { ',' });
                if (gameclick != null && gameclick.Length == 2)
                {
                    long tcurr = GameHelper.CurrentTimeMilisReal();
                    long tmem = long.Parse(gameclick[1]);
                    if ((tcurr - tmem) > (24 * 60 * 60000))
                    {
                        isovettime = true;
                    }
                    else
                    {
                        if (memclick.Length == 0)
                        {
                            memclick = listmemclick[i];
                        }
                        else
                        {
                            memclick += (";" + listmemclick[i]);
                        }
                    }
                }
            }
            if (isovettime)
            {
                PlayerPrefs.SetString("mem_game_promo_click", memclick);
            }
        }

        public List<PromoGameOb> getListGamePromo(int num)
        {
            string memlistgame = PlayerPrefs.GetString("cf_game_promo", "");
            string memclick = PlayerPrefs.GetString("mem_game_promo_click", "");
            if (memlistgame != null && memlistgame.Length > 10)
            {
                var obmemgames = (IDictionary<string, object>)JsonDecoder.DecodeText(memlistgame);
                List<object> listmemgames = null;
                List<PromoGameOb> listTmp = null;
                List<PromoGameOb> re = new List<PromoGameOb>();
                if (obmemgames != null && obmemgames.ContainsKey("games"))
                {
                    listmemgames = (List<object>)obmemgames["games"];
                    listTmp = new List<PromoGameOb>();
                }
                if (listmemgames != null)
                {
                    int idxproed = PlayerPrefs.GetInt("mem_idx_promo_ed", 0);
                    if (idxproed < 0)
                    {
                        idxproed = 0;
                    }
                    else if (idxproed >= listmemgames.Count)
                    {
                        idxproed = 0;
                    }
                    Debug.Log($"mysdk: getListGamePromo idxproed={idxproed}");
                    int idx = idxproed;
                    for (int run = 0; run < listmemgames.Count; run++)
                    {
                        var gamepromo = (IDictionary<string, object>)listmemgames[idx];
                        var gametmp = new PromoGameOb();
                        gametmp.pkg = (string)gamepromo["pkg"];
                        gametmp.name = (string)gamepromo["name"];
                        gametmp.appid = (string)gamepromo["appid"];
                        gametmp.setImg((string)gamepromo["icon"]);
                        gametmp.link = (string)gamepromo["link"];
                        gametmp.des = (string)gamepromo["des"];
                        if (!memclick.Contains(gametmp.pkg))
                        {
                            re.Add(gametmp);
                        }
                        else
                        {
                            listTmp.Add(gametmp);
                        }
                        idx++;
                        if (idx >= listmemgames.Count)
                        {
                            idx = 0;
                        }
                        if (re.Count >= num)
                        {
                            break;
                        }
                    }
                    PlayerPrefs.SetInt("mem_idx_promo_ed", idx);
                    if (re.Count < num && listTmp.Count > 0)
                    {
                        for (int i = re.Count; i < num; i++)
                        {
                            re.Add(listTmp[0]);
                            listTmp.RemoveAt(0);
                            if (listTmp.Count <= 0)
                            {
                                break;
                            }
                        }
                        listTmp.Clear();
                    }
                    if (re.Count > 0)
                    {
                        downLoadIconPromoGame(listmemgames, re[re.Count - 1].pkg);
                    }
                    return re;
                }
            }
            return null;
        }
        public void getListSkuApp()
        {
            string linkdownload = PlayerPrefs.GetString("mem_link_inapp", "");
            if (linkdownload.Length < 10 || linkdownload.StartsWith("105"))
            {
                return;
            }

            string url = linkdownload.Substring(3);
            if (!System.IO.Directory.Exists(Application.persistentDataPath + "/files/"))
            {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/files/");
            }

            string path = Application.persistentDataPath + "/files/cf_ina_ga.txt";
            DownLoadUtil.downloadText(url, path, (statedown) =>
            {
                if (statedown == 1)
                {
                    linkdownload = "105" + url;
                    PlayerPrefs.SetString("mem_link_inapp", linkdownload);
                }
            });
        }

    }

    public class QueueLogFir
    {
        public string nameevent;
        public Dictionary<string, object> memdic = null;
    }
}