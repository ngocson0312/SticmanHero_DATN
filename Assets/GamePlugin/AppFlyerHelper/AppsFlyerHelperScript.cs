//#define ENABLE_AppsFlyer
//#define AppsFlyer_IAPConnector
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_AppsFlyer
using AppsFlyerSDK;
#if AppsFlyer_IAPConnector
using AppsFlyerConnector;
#endif
#endif

namespace mygame.sdk
{

    // This class is intended to be used the the AppsFlyerObject.prefab

#if ENABLE_AppsFlyer
    public class AppsFlyerHelperScript : MonoBehaviour, IAppsFlyerConversionData
#else
    public class AppsFlyerHelperScript : MonoBehaviour
#endif
    {
        public static AppsFlyerHelperScript Instance = null;

        // These fields are set from the editor so do not modify!
        //******************************//
        public string devKey;
        public string appID;
        public string UWPAppID;
        public bool isDebug;
        public bool getConversionData;
        //******************************//

        int isLogCon = 1;

        private Action<bool> _callback;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
#if ENABLE_AppsFlyer
            // These fields are set from the editor so do not modify!
            //******************************//
            AppsFlyer.setHost("", "appsflyersdk.com");

            AppsFlyerAdRevenue.start();
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        AppsFlyer.initSDK(devKey, UWPAppID, getConversionData ? this : null);
#else
            AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
#endif
            AppsFlyer.setIsDebug(isDebug);
#if AppsFlyer_IAPConnector

#if UNITY_ANDROID
            AppsFlyerPurchaseConnector.init(this, AppsFlyerConnector.Store.GOOGLE);
            AppsFlyerPurchaseConnector.setIsSandbox(isDebug);
            AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions, AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
            AppsFlyerPurchaseConnector.build();
            AppsFlyerPurchaseConnector.startObservingTransactions();
#endif
#endif
            //******************************/

            AppsFlyer.startSDK();
#endif
        }

        public void setIapConnectorCB(Action<bool> cb)
        {
            _callback = cb;
        }

        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            Debug.Log("mysdk: AF didReceivePurchaseRevenueValidationInfo " + validationInfo);
            if (_callback != null)
            {
                if (validationInfo.Contains("success=true"))
                {
                    _callback(true);
                    _callback = null;
                }
                else
                {
                    _callback(false);
                    _callback = null;
                }
            }
        }

        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
#if ENABLE_AppsFlyer
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            string jsondata = "{";
            bool isbg = true;
            foreach (var item in conversionDataDictionary)
            {
                if (item.Key.Equals("media_source"))
                {
                    string va = (string)item.Value;
                    if (va.EndsWith("googleadwords_int"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.googleadwords_int;
                    }
                    else if (va.EndsWith("mintegral_int"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.mintegral_int;
                    }
                    else if (va.EndsWith("applovin_int"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.applovin_int;
                    }
                    else if (va.EndsWith("Facebook Ads"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.Facebook_Ads;
                    }
                    else if (va.EndsWith("unityads_int"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.unityads_int;
                    }
                    else if (va.EndsWith("af_cross_promotion"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.af_cross_promotion;
                    }
                    else if (va.EndsWith("ironsource_int"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.ironsource_int;
                    }
                    else if (va.EndsWith("bytedanceglobal_int"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.bytedanceglobal_int;
                    }
                    else if (va.EndsWith("restricted"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.googleadwords_int;
                    }
                    else if (va.EndsWith("Apple Search Ads"))
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.Apple_Search_Ads;
                    }
                    else
                    {
                        SDKManager.Instance.mediaType = MediaSourceType.UnKnown;
                    }
                    PlayerPrefsBase.Instance().setInt("mem_mediatype", (int)SDKManager.Instance.mediaType);
                }
                else
                {
                    if (item.Key.Equals("campaign"))
                    {
                        SDKManager.Instance.mediaCampain = (string)item.Value;
                        PlayerPrefsBase.Instance().setString("mem_mediacampain", SDKManager.Instance.mediaCampain);
                    }
                }
                if (isLogCon > 0)
                {
                    Debug.Log($"mysdk: AF data conversion key={item.Key}, value={item.Value}");
                    if (isbg)
                    {
                        isbg = false;
                        jsondata += "\"" + item.Key + " \":\"" + item.Value + "\"";
                    }
                    else
                    {
                        jsondata += ",\"" + item.Key + " \":\"" + item.Value + "\"";
                    }
                }
            }
            if (isLogCon > 0)
            {
                jsondata += "}";
                Myapi.LogEventApi.Instance().LogEvent(Myapi.MyEventLog.AdsMaxConversionData, jsondata);
                isLogCon--;
            }
            Debug.Log($"mysdk: AF mediaType={SDKManager.Instance.mediaType.ToString()}, meiaCampain={SDKManager.Instance.mediaCampain}");
#endif
        }

        public void onConversionDataFail(string error)
        {
#if ENABLE_AppsFlyer
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
#endif
        }

        public void onAppOpenAttribution(string attributionData)
        {
#if ENABLE_AppsFlyer
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
#endif
        }

        public void onAppOpenAttributionFailure(string error)
        {
#if ENABLE_AppsFlyer
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
#endif
        }

        public static void logPurchase(string sku, string value, string currency)
        {
#if ENABLE_AppsFlyer
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            dicParams.Add(AFInAppEvents.CONTENT_ID, sku);
            dicParams.Add(AFInAppEvents.CURRENCY, currency);
            dicParams.Add(AFInAppEvents.REVENUE, value);
            logEvent(AFInAppEvents.PURCHASE, dicParams);
#endif
        }

        public static void logEvent(string eventName, Dictionary<string, string> dicPrams = null)
        {
#if ENABLE_AppsFlyer
            if (dicPrams == null)
            {
                Dictionary<string, string> dicParams = new Dictionary<string, string>();
            }
            AppsFlyer.sendEvent(eventName, dicPrams);
#endif
        }
    }
}
