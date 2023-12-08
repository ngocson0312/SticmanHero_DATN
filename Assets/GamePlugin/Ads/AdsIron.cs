//#define ENABLE_ADS_IRON

using System;
using UnityEngine;
using System.Collections;

#if ENABLE_ADS_IRON

#endif

namespace mygame.sdk
{
    public class AdsIron : AdsBase
    {
#if ENABLE_ADS_IRON

#endif
        private static bool isInitAds = false;
        int statusLoadBanner = 0;
        int posBnCurr = -1;
        private bool isAdsInited = false;
        private bool isCallInit = false;

        public override void InitAds()
        {
#if ENABLE_ADS_IRON
            isEnable = true;
            if (!isInitAds)
            {
                isInitAds = true;
                isAdsInited = false;
                isCallInit = false;
                IronSource.Agent.setMetaData("do_not_sell", "true");

                int memage = PlayerPrefs.GetInt("mem_age_child", 0);
                if (memage >= 13)
                {
                    IronSource.Agent.setMetaData("is_child_directed", "false");
                }
                else if (memage < 13 && memage > 5)
                {
                    IronSource.Agent.setMetaData("is_child_directed", "true");
                    IronSource.Agent.setMetaData("is_deviceid_optout", "true");
                    IronSource.Agent.setConsent(false);
                }
                
                IronSource.Agent.shouldTrackNetworkState(true);
                int memss = PlayerPrefs.GetInt("mem_ss_consent_ir", -1);
                if (memss != -1)
                {
                    if (memss == 1)
                    {
                        IronSource.Agent.setConsent(true);
                    }
                    else
                    {
                        IronSource.Agent.setConsent(false);
                    }
                    checkInit();
                }
                else
                {
                    int showss = PlayerPrefs.GetInt("mem_show_CMP", 0);
                    if (showss == 1)
                    {
                        checkInit();
                    }
                    else
                    {
                        StartCoroutine(WaitInit());
                    }
                }
            }
#endif
        }

        IEnumerator WaitInit()
        {
            yield return new WaitForSeconds(30);
            checkInit();
        }

        private void checkInit()
        {
            if (!isCallInit)
            {
                isCallInit = true;
#if ENABLE_ADS_IRON
                IronSource.Agent.init(appId);
#endif
            }
        }

        public override void AdsAwake()
        {
#if ENABLE_ADS_IRON
            GameAdsHelperBridge.CBRequestGDPR += onShowCmp;
            isEnable = true;
#endif
        }

        public void onShowCmp(int state, string des)
        {
#if ENABLE_ADS_IRON
            if (state == 0)
            {
                Debug.Log($"mysdk: ads iron onshow cmp");
                if (des != null && des.CompareTo("0") == 0)
                {
                    checkInit();
                }
            }
            else if (state == 1)
            {
                if (des != null && des.Length > 5)
                {
                    PlayerPrefs.SetInt("mem_ss_consent_ir", 1);
                    IronSource.Agent.setConsent(true);
                }
                else
                {
                    PlayerPrefs.SetInt("mem_ss_consent_ir", 0);
                    IronSource.Agent.setConsent(false);
                }
                checkInit();
            }
#endif
        }

        public override string getname()
        {
            return "iron";
        }

        private void Start()
        {
#if ENABLE_ADS_IRON
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

            IronSourceEvents.onImpressionDataReadyEvent += irOnImpressionDataReadyEvent;

            IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadFailedEvent;
            //IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            //IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            //IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            //IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialAdClosedEvent;

            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += ReWardedVideoOnAdAvailableEvent;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
#endif
        }

        private void SdkInitializationCompletedEvent()
        {
            Debug.Log($"mysdk: ads iron SdkInitializationCompletedEvent");
            isAdsInited = true;
            AdsProcessCB.Instance().Enqueue(() =>
            {
                advhelper.loadFull4ThisTurn(false, 99, false, null);
                advhelper.loadGift4ThisTurn(99, null);
            }, 0.1f);
        }

        void OnApplicationPause(bool isPaused)
        {
#if ENABLE_ADS_IRON
            IronSource.Agent.onApplicationPause(isPaused);
#endif
        }

        protected override void tryLoadBanner(int type)
        {
            type = 0;
#if ENABLE_ADS_IRON
            SdkUtil.logd("ads iron tryLoadBanner");
            if (!isAdsInited)
            {
                SdkUtil.logd($"ads iron tryLoadBanner not init");
                var tmpcb = cbBanner;
                cbBanner = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
                return;
            }
            if (BNTryLoad >= toTryLoad)
            {
                SdkUtil.logd("ads iron tryLoadBanner over trycount");
                if (cbBanner != null)
                {
                    var tmpcb = cbBanner;
                    cbBanner = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }
                if (advhelper != null)
                {
                    advhelper.onBannerLoadFail(adsType);
                }
                return;
            }
            SdkUtil.logd("ads iron tryLoadBanner load");
            dicBanner[type].isBNLoading = true;
            dicBanner[type].isBNloaded = false;
            dicBanner[type].isShowing = false;
            IronSourceBannerPosition adposbn;
            if (dicBanner[type].posBanner == 0)
            {
                adposbn = IronSourceBannerPosition.TOP;
            }
            else if (dicBanner[type].posBanner == 1)
            {
                adposbn = IronSourceBannerPosition.BOTTOM;
            }
            else
            {
                adposbn = IronSourceBannerPosition.BOTTOM;
            }
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, adposbn);
            if (statusLoadBanner == 0)
            {
                statusLoadBanner = 1;
            }
            StartCoroutine(waitCheckLoadBannerErr());
#else
            if (cbBanner != null)
            {
                SdkUtil.logd("ads iron tryLoadBanner not enable");
                var tmpcb = cbBanner;
                cbBanner = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }

        IEnumerator waitCheckLoadBannerErr()
        {
            yield return new WaitForSeconds(90);
            if (statusLoadBanner == 1)
            {
                statusLoadBanner = 0;
                destroyBanner(0);
                SdkUtil.logd("ads iron waitCheckLoadBannerErr");
                dicBanner[0].isBNloaded = false;
                dicBanner[0].isBNLoading = false;
                BNTryLoad++;
                tryLoadBanner(0);
            }
        }
        public override void loadBanner(int type, AdCallBack cb)
        {
            SdkUtil.logd("ads iron loadBanner");
            cbBanner = cb;
            type = 0;
            if (!dicBanner[type].isBNLoading)
            {
                BNTryLoad = 0;
                tryLoadBanner(type);
            }
            else
            {
                SdkUtil.logd("ads iron loadBanner isloading");
            }
        }
        public override bool showBanner(int type, int pos, int width, AdCallBack cb, float dxCenter)
        {
            SdkUtil.logd("ads iron showBanner");
            type = 0;
            if (posBnCurr != -1 && posBnCurr != pos)
            {
                destroyBanner(type);
            }
#if ENABLE_ADS_IRON
            dicBanner[type].isBNShow = true;
            dicBanner[type].posBanner = pos;
            bnWidth = width;
            if (dicBanner[type].isBNloaded)
            {
                if (!dicBanner[type].isShowing)
                {
                    dicBanner[type].isShowing = true;
                    IronSource.Agent.displayBanner();
                }
                return true;
            }
            else
            {
                SdkUtil.logd("ads iron showBanner not show and load");
                posBnCurr = pos;
                loadBanner(type, cb);
                return false;
            }
#else
            if (cb != null)
            {
                SdkUtil.logd("ads iron tryLoadBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
#endif
        }

        public override bool showBanner(int type, Rect poscustom, AdCallBack cb)
        {
            type = 0;
            if (cb != null)
            {
                SdkUtil.logd("ads iron tryLoadBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
        }
        public override void hideBanner(int type)
        {
            SdkUtil.logd("ads iron hideBanner");
            type = 0;
            dicBanner[type].isBNShow = false;
            dicBanner[type].isShowing = false;
#if ENABLE_ADS_IRON
            IronSource.Agent.hideBanner();
#endif
        }
        public override void destroyBanner(int type)
        {
            SdkUtil.logd("ads iron destroyBanner");
            type = 0;
            dicBanner[type].isBNLoading = false;
            dicBanner[type].isBNloaded = false;
            posBnCurr = -1;
#if ENABLE_ADS_IRON
            IronSource.Agent.destroyBanner();
#endif
        }

        //
        protected override void tryLoadNative()
        { }
        public override void loadNative(AdCallBack cb)
        {
            if (cb != null)
            {
                cb(AD_State.AD_LOAD_FAIL);
            }
        }
        public override bool showNative(Rect pos, AdCallBack cb)
        {
            if (cb != null)
            {
                SdkUtil.logd("ads iron tryLoadBanner not enable");
                cb(AD_State.AD_LOAD_FAIL);
            }
            return false;
        }
        public override void hideNative()
        { }

        //
        public override void clearCurrFull()
        {
            if (getFullLoaded(false) == 1)
            {
#if ENABLE_ADS_IRON
                isFullLoaded = false;
#endif
            }
        }
        public override int getFullLoaded(bool isOpenads)
        {
            if (isFullLoaded)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        protected override void tryLoadFull(bool isSplash)
        {
#if ENABLE_ADS_IRON
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron tryLoadFull =" + fullId);
#endif
            if (!isAdsInited)
            {
#if ENABLE_MYLOG
                SdkUtil.logd($"ads iron tryLoadFull not init");
#endif
                if (cbFullLoad != null)
                {
                    var tmpcb = cbFullLoad;
                    cbFullLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            if (FullTryLoad >= toTryLoad)
            {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron tryLoadFull over try");
#endif
                if (cbFullLoad != null)
                {
                    var tmpcb = cbFullLoad;
                    cbFullLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }
                return;
            }
            isFullLoading = true;
            isFullLoaded = false;
            IronSource.Agent.loadInterstitial();
#else
            if (cbFullLoad != null)
            {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron tryLoadFull not enable");
#endif
                var tmpcb = cbFullLoad;
                cbFullLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif
        }
        public override void loadFull(bool isSplash, AdCallBack cb)
        {
            SdkUtil.logd("ads iron loadFull");
            cbFullLoad = cb;
            if (!isFullLoading && !isFullLoaded)
            {
                SdkUtil.logd("ads iron loadFull load");
                FullTryLoad = 0;
                this.isSplash = isSplash;
                tryLoadFull(isSplash);
            }
            else
            {
                SdkUtil.logd("ads iron loadFull loading=" + isFullLoading + " or loaded=" + isFullLoaded);
            }
        }
        public override bool showFull(bool isOpenAds, AdCallBack cb)
        {
            SdkUtil.logd("ads iron showFull");
            cbFullShow = null;
            if (getFullLoaded(isOpenAds) > 0)
            {
                SdkUtil.logd("ads iron showFull show");
                FullTryLoad = 0;
#if ENABLE_ADS_IRON
                cbFullShow = cb;
                IronSource.Agent.showInterstitial();
                return true;
#endif
            }
            return false;
        }

        //------------------------------------------------
        public override void clearCurrGift()
        {
            if (getGiftLoaded())
            {
#if ENABLE_ADS_IRON
#endif
            }
        }
        public override bool getGiftLoaded()
        {
#if ENABLE_ADS_IRON
            return (isGiftLoaded);
#endif
            return false;
        }
        protected override void tryloadGift()
        {
#if ENABLE_ADS_IRON
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron Request gift =" + giftId);
#endif
            isGiftLoading = true;
            //IronSource.Agent.loadRewardedVideo();
            StartCoroutine(waitGiftReady());
#else
            if (cbGiftLoad != null)
            {
                var tmpcb = cbGiftLoad;
                cbGiftLoad = null;
                tmpcb(AD_State.AD_LOAD_FAIL);
            }
#endif      
        }
        IEnumerator waitGiftReady()
        {
#if ENABLE_ADS_IRON
            int count = 0;
            while (!isGiftLoaded && isGiftLoading && count < 40)
            {
                yield return new WaitForSeconds(0.5f);
                count++;
            }
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron waitGiftReady");
#endif
            isGiftLoading = false;
            if (cbGiftLoad != null)
            {
                if (isGiftLoaded)
                {
                    var tmpcb = cbGiftLoad;
                    cbGiftLoad = null;
                    tmpcb(AD_State.AD_LOAD_OK);
                }
                else
                {
                    var tmpcb = cbGiftLoad;
                    cbGiftLoad = null;
                    tmpcb(AD_State.AD_LOAD_FAIL);
                }
            }
#else
            yield return null;
#endif
        }
        public override void loadGift(AdCallBack cb)
        {
            SdkUtil.logd("ads iron loadGift");
            cbGiftLoad = cb;
            if (!isGiftLoading && !isGiftLoaded)
            {
                SdkUtil.logd("ads iron loadGift load");
                GiftTryLoad = 0;
                tryloadGift();
            }
            else
            {
                SdkUtil.logd("ads iron loadGift loading=" + isGiftLoading + " or loaded=" + isGiftLoaded);
            }
        }
        public override bool showGift(AdCallBack cb)
        {
            SdkUtil.logd("ads iron showGift");
#if ENABLE_ADS_IRON
            if (getGiftLoaded() && IronSource.Agent.isRewardedVideoAvailable())
#else
            if (getGiftLoaded())
#endif
            {
                SdkUtil.logd("ads iron showGift show");
                cbGiftShow = cb;
                isGiftLoaded = false;
#if ENABLE_ADS_IRON
                IronSource.Agent.showRewardedVideo();
                return true;
#endif
            }
            return false;
        }

        //-------------------------------------------------------------------------------
#if ENABLE_ADS_IRON

        int countBnTo = 0;
        int countBnAdmob = 0;
        int countFullTo = 0;
        int countFullAdmob = 0;
        int countGiftTo = 0;
        int countGiftAdmob = 0;
        private void irOnImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                FIRhelper.logEventAdsPaidIron(appId, impressionData.adNetwork, impressionData.adUnit, impressionData.instanceName, (double)impressionData.revenue, impressionData.country, impressionData.placement);
                if (impressionData.adUnit.Contains("interstitial"))
                {
                    countFullTo++;
                    if (impressionData.adNetwork.Contains("admob"))
                    {
                        countFullAdmob++;
                    }
                }
                else if (impressionData.adUnit.Contains("rewarded_video"))
                {
                    countGiftTo++;
                    if (impressionData.adNetwork.Contains("admob"))
                    {
                        countGiftAdmob++;
                    }
                }
                else if (impressionData.adUnit.Contains("banner"))
                {
                    countBnTo++;
                    if (impressionData.adNetwork.Contains("admob"))
                    {
                        countBnAdmob++;
                    }
                }
                Debug.Log($"mysdk:{impressionData.adNetwork}-{impressionData.adUnit} Banner:{countBnAdmob}-{countBnTo}, full:{countFullAdmob}-{countFullTo}, Gift:{countGiftAdmob}-{countGiftTo}");
            }
        }

        #region BANNER AD EVENTS

        private void BannerAdLoadedEvent(IronSourceAdInfo adInfo)
        {

            SdkUtil.logd("ads iron HandleBannerAdLoaded");
            dicBanner[0].isBNloaded = true;
            dicBanner[0].isBNLoading = false;
            BNTryLoad = 0;
            statusLoadBanner = -1;

            if (dicBanner[0].isBNShow)
            {
                SdkUtil.logd("ads iron BannerAdLoaded show");
                if (!dicBanner[0].isShowing)
                {
                    dicBanner[0].isShowing = true;
                    IronSource.Agent.displayBanner();
                }
            }
            else
            {
                SdkUtil.logd("ads iron BannerAdLoaded hide");
                dicBanner[0].isShowing = false;
                IronSource.Agent.hideBanner();
            }

            if (cbBanner != null)
            {
                var tmpcb = cbBanner;
                cbBanner = null;
                tmpcb(AD_State.AD_LOAD_OK);
            }
            if (advhelper != null)
            {
                advhelper.onBannerLoadOk(adsType);
            }
        }

        private void BannerAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            SdkUtil.logd("ads iron HandleBannerAdFailedToLoad");
            statusLoadBanner = -1;
            if (!dicBanner[0].isBNloaded)
            {
                SdkUtil.logd("ads iron HandleBannerAdFailedToLoad 1");
                dicBanner[0].isBNloaded = false;
                dicBanner[0].isBNLoading = false;
                AdsProcessCB.Instance().Enqueue(() => 
                {
                    BNTryLoad++;
                    tryLoadBanner(0);
                }, 1.0f);
            }
        }
        #endregion

        #region INTERSTITIAL AD EVENTS

        private void InterstitialAdReadyEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron HandleInterstitialAdDidLoad");
#endif
            FullTryLoad = 0;
            isFullLoading = false;
            isFullLoaded = true;
            if (cbFullLoad != null)
            {
                var tmpcb = cbFullLoad;
                cbFullLoad = null;
                tmpcb(AD_State.AD_LOAD_OK);
            }
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron HandleInterstitialAdDidLoad1");
#endif
        }

        private void InterstitialAdLoadFailedEvent(IronSourceError ironSourceError)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron HandleInterstitialAdDidFailWithError");
#endif
            isFullLoading = false;
            isFullLoaded = false;
            AdsProcessCB.Instance().Enqueue(() =>
            {
                FullTryLoad++;
                tryLoadFull(isSplash);
            }, 1.0f);
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron HandleInterstitialAdDidFailWithError1");
#endif
        }

        private void InterstitialAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron InterstitialAdShowSucceededEvent");
#endif
        }

        private void InterstitialAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron InterstitialAdShowFailedEvent err=" + ironSourceError.ToString());
#endif
            isFullLoading = false;
            isFullLoaded = false;
            if (cbFullShow != null)
            {
                AdCallBack tmpcb = cbFullShow;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron full InterstitialAdShowFailedEvent 1");
#endif
                AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
            }
            onFullClose();
        }

        private void InterstitialAdClickedEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron InterstitialAdClickedEvent");
#endif
        }

        private void InterstitialAdOpenedEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron InterstitialAdOpenedEvent 0");
#endif
            if (cbFullShow != null)
            {
                AdCallBack tmpcb = cbFullShow;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron full InterstitialAdOpenedEvent 1");
#endif
                AdsProcessCB.Instance().Enqueue(() =>
                {
                    tmpcb(AD_State.AD_SHOW);
                });
            }
        }

        private void InterstitialAdClosedEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron InterstitialAdClosedEvent 0");
#endif
            isFullLoading = false;
            isFullLoaded = false;
            if (cbFullShow != null)
            {
                AdCallBack tmpcb = cbFullShow;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron full InterstitialAdClosedEvent 1");
#endif
                AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
            }
            onFullClose();

            FullTryLoad = 0;
            cbFullShow = null;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron HandleInterstitialClosed 2");
#endif
        }
        #endregion

        #region REWARDED VIDEO AD EVENTS
        private void ReWardedVideoOnAdAvailableEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw ReWardedVideoOnAdAvailableEvent");
#endif
            GiftTryLoad = 0;
            isGiftLoading = false;
            isGiftLoaded = true;
            if (cbGiftLoad != null)
            {
                var tmpcb = cbGiftLoad;
                cbGiftLoad = null;
                tmpcb(AD_State.AD_LOAD_OK);
            }
        }

        private void RewardedVideoOnAdUnavailable()
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw RewardedVideoOnAdUnavailable");
#endif
            if (isGiftLoading)
            {
                isGiftLoading = false;
                isGiftLoaded = false;
#if ENABLE_MYLOG
                SdkUtil.logd("ads iron rw RewardedVideoOnAdUnavailable set fail");
#endif
            }
            //AdsProcessCB.Instance().Enqueue(() =>
            //{
            //    GiftTryLoad++;
            //    tryloadGift();
            //}, 1.0f);
        }

        private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw RewardedVideoOnAdClickedEvent");
#endif
        }

        private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw HandleRewardBasedVideoRewarded");
#endif
            isRewardCom = true;
        }

        private void RewardedVideoAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw RewardedVideoAdShowFailedEvent err=" + error.ToString());
#endif
            if (cbGiftShow != null)
            {
                AdCallBack tmpcb = cbGiftShow;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw _cbAD fail");
#endif
                AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_SHOW_FAIL); });
            }
            onGiftClose();
        }

        private void RewardedVideoAdOpenedEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw RewardedVideoAdOpenedEvent");
#endif
            if (cbGiftShow != null)
            {
                AdCallBack tmpcb = cbGiftShow;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw RewardedVideoAdOpenedEvent 1");
#endif
                AdsProcessCB.Instance().Enqueue(() =>
                {
                    tmpcb(AD_State.AD_SHOW);
                });
            }
        }

        private void RewardedVideoAdClosedEvent(IronSourceAdInfo adInfo)
        {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw HandleRewardBasedVideoClosed");
#endif
            if (cbGiftShow != null)
            {
                AdCallBack tmpcb = cbGiftShow;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw _cbAD != null");
#endif
                if (isRewardCom)
                {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw _cbAD reward");
#endif
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_OK); });
                }
                else
                {
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw _cbAD fail");
#endif
                    AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_REWARD_FAIL); });
                }
                AdsProcessCB.Instance().Enqueue(() => { tmpcb(AD_State.AD_CLOSE); });
            }
            onGiftClose();

            isRewardCom = false;
            GiftTryLoad = 0;
            cbGiftShow = null;
#if ENABLE_MYLOG
            SdkUtil.logd("ads iron rw HandleRewardBasedVideoClosed3");
#endif
        }

        #endregion

#endif

    }
}