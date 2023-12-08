using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using mygame.sdk;
using MyJson;
#if UNITY_ANDROID
using mygame.plugin.Android;
#endif

namespace mygame.sdk
{
    public class MyAdsOpen : MonoBehaviour
    {
        protected AdCallBack cbLoad;
        protected AdCallBack cbShow;

        private GameObject PopupAdsOpen;

        public bool isLoading = false;
        public bool isLoaded = false;
        int idxShow = 0;
        bool isStartCall = false;
        MyOpenAdsOb currAds;

        public List<MyOpenAdsOb> ListMyAdsOpen = new List<MyOpenAdsOb>();

        public static MyAdsOpen Instance { get; private set; }

#if UNITY_ANDROID
#endif

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            if (isStartCall)
            {
                if (SDKManager.Instance.PopupShowFirstAds.transform.parent.Find("EventSystem") != null)
                {
                    SDKManager.Instance.PopupShowFirstAds.transform.parent.Find("EventSystem").gameObject.SetActive(true);
                }
            }
        }

        private void Start()
        {
            if (SDKManager.Instance.PopupShowFirstAds.transform.parent.Find("EventSystem") != null)
            {
                SDKManager.Instance.PopupShowFirstAds.transform.parent.Find("EventSystem").gameObject.SetActive(true);
            }
            isStartCall = true;
        }
         
        private void OnDisable()
        {
            if (SDKManager.Instance.PopupShowFirstAds.transform.parent.Find("EventSystem") != null)
            {
                SDKManager.Instance.PopupShowFirstAds.transform.parent.Find("EventSystem").gameObject.SetActive(false);
            }
        }

        public bool hasAds()
        {
            return (ListMyAdsOpen != null && ListMyAdsOpen.Count > 0);
        }

        public bool isShow()
        {
            if (hasAds() && isLoaded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region My ads open implementation
        public void load(AdCallBack cb = null)
        {
            SdkUtil.logd($"MyAdsOpen load isLoading={isLoading}, isLoaded={isLoaded}");
            if (ListMyAdsOpen.Count > 0)
            {
                if (!isLoading && !isLoaded)
                {
                    if (idxShow >= ListMyAdsOpen.Count)
                    {
                        idxShow = 0;
                    }
                    currAds = ListMyAdsOpen[idxShow];
                    if (currAds.texture != null)
                    {
                        Resources.UnloadAsset(currAds.texture);
                        currAds.texture = null;
                    }
                    idxShow++;
                    if (idxShow >= ListMyAdsOpen.Count)
                    {
                        idxShow = 0;
                    }
                    isLoaded = false;
                    isLoading = true;
                    loadimageTexture(currAds.linkAds, (tt) =>
                    {
                        isLoading = false;
                        if (tt != null)
                        {
                            isLoaded = true;
                            currAds.texture = tt;
                            if (cb != null)
                                cb(AD_State.AD_LOAD_OK);
                        }
                        else
                        {
                            if (cb != null)
                                cb(AD_State.AD_LOAD_FAIL);
                        }
                    });
                }
                else
                {
                    if (isLoaded)
                    {
                        if (cb != null)
                            cb(AD_State.AD_LOAD_OK);
                    }
                    else if (isLoading)
                    {
                        SdkUtil.logd($"MyAdsOpen load isloading");
                    }
                }
            }
            else
            {
                isLoading = false;
                isLoaded = false;
                if (cb != null)
                {
                    cb(AD_State.AD_LOAD_FAIL);
                }
            }
        }

        public bool show(AdCallBack cb)
        {
            SdkUtil.logd($"MyAdsOpen show isLoaded={isLoaded} call");
            FIRhelper.logEvent("show_ads_myopen_call");
            if (isLoaded)
            {
                cbShow = cb;
                isLoaded = false;
                if (PopupAdsOpen == null)
                {
                    var prefab = Resources.Load<GameObject>("Popup/PopupMyopenAds");
                    PopupAdsOpen = Instantiate(prefab, Vector3.zero, Quaternion.identity, SDKManager.Instance.PopupShowFirstAds.transform.parent);
                }
                
                PopupAdsOpen.GetComponent<PopupMyOpenAds>().initAds(currAds, (status) =>
                {
                    if (status == 1)
                    {
                        if (cb != null)
                        {
                            cb(AD_State.AD_SHOW);
                        }
                    }
                    else if (status == 2)
                    {
                        AdsHelper.Instance.onHideMyAdsOpen();
                        if (cb != null)
                        {
                            cb(AD_State.AD_CLOSE);
                        }
                    }

                });
                PopupAdsOpen.SetActive(true);
                RectTransform rc = PopupAdsOpen.GetComponent<RectTransform>();
                rc.anchorMin = new Vector2(0, 0);
                rc.anchorMax = new Vector2(1, 1);
                rc.sizeDelta = new Vector2(0, 0);
                rc.anchoredPosition = Vector2.zero;
                rc.anchoredPosition3D = Vector3.zero;
                AdsHelper.Instance.onShowMyAdsOpen();
                FIRhelper.logEvent("show_ads_myopen_show");
                return true;
            }
            else
            {
                return false;
            }
        }

        void loadimageTexture(string url, Action<Texture2D> cb)
        {
            StartCoroutine(coDownloadImage(url, cb));
        }

        private IEnumerator coDownloadImage(string imageUrl, Action<Texture2D> cb)
        {
            SdkUtil.logd("MyAdsOpen coDownloadImage: imageUrl=" + imageUrl);
            string pathimg = ImageLoader.url2nameData(imageUrl, 1);
            Texture2D tt = null;
            if (File.Exists(DownLoadUtil.pathCache() + "/" + pathimg))
            {
                yield return new WaitForSeconds(0.01f);
                if (cb != null)
                {
                    tt = ImageLoader.Instance.loadTexttureFromCache(pathimg, 200, 350);
                    cb(tt);
                }
            }
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Texture2D tt2d = (Texture2D)myTexture;
                    if (tt2d != null)
                    {
                        SdkUtil.logd("MyAdsOpen coDownloadImage: Success");
                        if (cb != null)
                        {
                            cb(tt2d);
                        }
                        ImageLoader.Instance.saveTextturetocache(tt2d, pathimg);
                    }
                    else
                    {
                        SdkUtil.logd("MyAdsOpen coDownloadImage: err");
                        if (cb != null)
                        {
                            cb(null);
                        }
                    }
                }
                else
                {
                    SdkUtil.logd("MyAdsOpen coDownloadImage err=" + www.error);
                    if (cb != null)
                    {
                        cb(null);
                    }
                }
            }
        }

        #endregion

        public void loadMyOpenAds()
        {
            string data = PlayerPrefs.GetString("mem_data_myopen_ad", "");
            if (data != null && data.Length > 0)
            {
                var obupdategame = (IDictionary<string, object>)JsonDecoder.DecodeText(data);
                if (obupdategame != null || obupdategame.Count > 0)
                {
                    if (obupdategame.ContainsKey("games"))
                    {
                        ListMyAdsOpen.Clear();
                        var games = (List<object>)obupdategame["games"];
                        foreach (var itemgame in games)
                        {
                            var gameopen = (IDictionary<string, object>)itemgame;
                            MyOpenAdsOb myads = new MyOpenAdsOb();
                            ListMyAdsOpen.Add(myads);

                            myads.linkAds = (string)gameopen["linkAds"];
                            myads.gameId = (string)gameopen["gameId"];
                        }
                    }
                }
            }
        }
    }
}
