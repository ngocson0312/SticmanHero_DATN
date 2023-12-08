using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using mygame.sdk;
using MyJson;

public class ObjectAdsCf
{
    public string countrycode = mygame.sdk.GameHelper.CountryDefault;

    public int OpenAdShowtype = 4;
    public int OpenAdShowat;
    public int OpenAdIsShowFirstOpen;
    public int OpenAdDelTimeOpen;
    public int OpenadLvshow;
    public int OpenAdTimeWaitShowFirst;
    public int cfLvStaticAds;
    public int typeMyopenAd;

    public int isChangeBNStatic;
    public int verShowBanner;
    public string stepShowBanner;
    public string stepFloorECPMBanner;
    public List<int> bnStepShowCircle;
    public List<int> bnStepShowRe;

    public List<int> nativeStepShow;

    public int fullTotalOfday;
    public int fullLevelStart;
    public int fullSessionStart;
    public int full2LevelStart;
    public int fullTimeStart;
    public int fullDeltatime;
    public int full2Deltatime;
    public int fullShowPlaying;
    public int fullLoadAdsMobStatic;
    public int fullDefaultNumover = 2;
    public int full2Numover = 2;
    public string stepShowFull;
    public string stepShowFull2;
    public string intervalnumoverfull;
    public string stepFloorECPMFull;
    public string stepFloorECPMGift;
    public string excluseFullrunning;
    public List<IntervalLevelShowfull> listIntervalShow = new List<IntervalLevelShowfull>();
    public List<int> fullStepShowCircle;
    public List<int> fullStepShowRe;
    public List<int> fullExcluseShowRunning;
    public List<int> full2StepShowCircle;
    public int isLoadMaxLow;

    public int maskAdsStatus = 0xf;

    public int giftTotalOfday;
    public int giftDeltatime;
    public string stepShowGift;
    public int stateShowAppLlovin = 0;//=1 wait time until level to show; =2 pass lv to show
    public int levelShowAppLovin = 0;
    public int tryloadApplovin;
    public int loadOtherWhenLoadedApplovinButNotReady = 0;
    public List<int> giftStepShowCircle;
    public List<int> giftStepShowRe;

    public int specialType;// =0-session, 1-timeplay
    public string specialData = "";
    public List<SpecialConditionShow> listSpecialShow = new List<SpecialConditionShow>();

    public int typeLoadStart = 1;//0-auto, 1-video-auto, 2-video only

    public ObjectAdsCf(string code = mygame.sdk.GameHelper.CountryDefault)
    {
        bnStepShowCircle = new List<int>();
        bnStepShowRe = new List<int>();

        nativeStepShow = new List<int>();

        fullStepShowCircle = new List<int>();
        fullStepShowRe = new List<int>();
        fullExcluseShowRunning = new List<int>();
        full2StepShowCircle = new List<int>();
        listIntervalShow.Clear();

        giftStepShowCircle = new List<int>();
        giftStepShowRe = new List<int>();

        listSpecialShow.Clear();

        countrycode = code;
        //loadFromPlayerPrefs();
    }

    public void coppyFromOther(ObjectAdsCf other)
    {
        if (other != null)
        {
            Debug.Log($"mysdk: object ads coppy {other.countrycode} to {countrycode}");
            maskAdsStatus = other.maskAdsStatus;
            isChangeBNStatic = other.isChangeBNStatic;
            stepShowBanner = other.stepShowBanner;
            parSerStepBN(stepShowBanner);
            if (bnStepShowCircle.Count == 0 && bnStepShowRe.Count == 0)
            {
                parSerStepBN(AppConfig.defaultStepBanner);
            }
            string sstepnative = PlayerPrefs.GetString("cf_step_native", "0,10");
            parSerStepNative(sstepnative);

            fullTotalOfday = other.fullTotalOfday;
            fullLevelStart = other.fullLevelStart;
            fullSessionStart = other.fullSessionStart;
            full2LevelStart = other.full2LevelStart;
            fullTimeStart = other.fullTimeStart;
            fullDefaultNumover = other.fullDefaultNumover;
            full2Numover = other.full2Numover;
            intervalnumoverfull = other.intervalnumoverfull;
            fullShowPlaying = other.fullShowPlaying;
            fullLoadAdsMobStatic = other.fullLoadAdsMobStatic;
            parserIntervalnumoverfull(intervalnumoverfull);
            fullDeltatime = other.fullDeltatime;
            full2Deltatime = other.full2Deltatime;
            typeLoadStart = other.typeLoadStart;
            stepShowFull = other.stepShowFull;
            stepShowFull2 = other.stepShowFull2;
            isLoadMaxLow = other.isLoadMaxLow;
            parSerStepFull(stepShowFull);
            parSerStepFull2(stepShowFull2);
            if (fullStepShowCircle.Count == 0 && fullStepShowRe.Count == 0)
            {
                parSerStepFull(AppConfig.defaultStepFull);
            }
            excluseFullrunning = other.excluseFullrunning;
            parSerExcluseFull(excluseFullrunning);

            giftTotalOfday = other.giftTotalOfday;
            giftDeltatime = other.giftDeltatime;
            stepShowGift = other.stepShowGift;
            parSerStepGift(stepShowGift);
            if (giftStepShowCircle.Count == 0 && giftStepShowRe.Count == 0)
            {
                parSerStepGift(AppConfig.defaultStepGift);
            }

            if (AdsHelper.Instance != null)
            {
                OpenAdShowtype = 4;
            }
            OpenAdShowtype = other.OpenAdShowtype;
            OpenAdShowat = other.OpenAdShowat;
            OpenAdIsShowFirstOpen = other.OpenAdIsShowFirstOpen;
            OpenAdDelTimeOpen = other.OpenAdDelTimeOpen;
            OpenadLvshow = other.OpenadLvshow;
            OpenAdTimeWaitShowFirst = other.OpenAdTimeWaitShowFirst;

            cfLvStaticAds = other.cfLvStaticAds;
            typeMyopenAd = other.typeMyopenAd;

            stateShowAppLlovin = other.stateShowAppLlovin;
            levelShowAppLovin = other.levelShowAppLovin;
            tryloadApplovin = other.tryloadApplovin;
            loadOtherWhenLoadedApplovinButNotReady = other.loadOtherWhenLoadedApplovinButNotReady;

            stepFloorECPMBanner = other.stepFloorECPMBanner;
            stepFloorECPMFull = other.stepFloorECPMFull;
            stepFloorECPMGift = other.stepFloorECPMGift;

            specialType = other.specialType;
            specialData = other.specialData;
            parSerSpecialStep(specialData);
        }
    }

    public void loadFromPlayerPrefs()
    {
        maskAdsStatus = PlayerPrefs.GetInt("cf_mask_ads_status", 0xf);
        if (AppConfig.isAddAdsMob)
        {
            isChangeBNStatic = PlayerPrefs.GetInt("cf_change_bn_sta", 1);
        }
        else
        {
            isChangeBNStatic = PlayerPrefs.GetInt("cf_change_bn_sta", 0);
        }
        stepShowBanner = PlayerPrefs.GetString("cf_step_banner", AppConfig.defaultStepBanner);
        parSerStepBN(stepShowBanner);
        if (bnStepShowCircle.Count == 0 && bnStepShowRe.Count == 0)
        {
            parSerStepBN(AppConfig.defaultStepBanner);
        }
        string sstepnative = PlayerPrefs.GetString("cf_step_native", "0,10");
        parSerStepNative(sstepnative);

        fullTotalOfday = PlayerPrefs.GetInt("cf_fullTotalOfday", 200);
        fullLevelStart = PlayerPrefs.GetInt("cf_fullLevelStart", 1);
        fullSessionStart = PlayerPrefs.GetInt("cf_fullSessionStart", 1);
        full2LevelStart = PlayerPrefs.GetInt("cf_full2LevelStart", 2000);
        fullTimeStart = PlayerPrefs.GetInt("cf_fullTimeStart", 0) * 1000;
        fullDefaultNumover = PlayerPrefs.GetInt("cf_fulldefaultnumover", 1);
        full2Numover = PlayerPrefs.GetInt("cf_full2numover", 2);
        intervalnumoverfull = PlayerPrefs.GetString("cf_fullNumover", "");
        parserIntervalnumoverfull(intervalnumoverfull);
        fullDeltatime = PlayerPrefs.GetInt("cf_fullDeltatime", 30000);
        full2Deltatime = PlayerPrefs.GetInt("cf_full2Deltatime", 150000);
        typeLoadStart = PlayerPrefs.GetInt("cf_type_full_start", 1);
        fullShowPlaying = PlayerPrefs.GetInt("cf_fullShowPlaying", 0);
        isLoadMaxLow = PlayerPrefs.GetInt("cf_load_maxlow", 0);
        if (AppConfig.isAddAdsMob)
        {
            fullLoadAdsMobStatic = PlayerPrefs.GetInt("cf_fullLoadAdsMobStatic", 1);
        }
        else
        {
            fullLoadAdsMobStatic = PlayerPrefs.GetInt("cf_fullLoadAdsMobStatic", 0);
        }
        stepShowFull = PlayerPrefs.GetString("cf_step_full", AppConfig.defaultStepFull);
        stepShowFull2 = PlayerPrefs.GetString("cf_step_full2", "");
        parSerStepFull(stepShowFull);
        parSerStepFull2(stepShowFull2);
        if (fullStepShowCircle.Count == 0 && fullStepShowRe.Count == 0)
        {
            parSerStepFull(AppConfig.defaultStepFull);
        }
        excluseFullrunning = PlayerPrefs.GetString("cf_full_excluse_run", AppConfig.defaultExcluseFull);
        parSerExcluseFull(excluseFullrunning);

        giftTotalOfday = PlayerPrefs.GetInt("cf_giftTotalOfday", 200);
        giftDeltatime = PlayerPrefs.GetInt("cf_giftDeltatime", 5000);
        stepShowGift = PlayerPrefs.GetString("cf_step_gift", AppConfig.defaultStepGift);
        parSerStepGift(stepShowGift);
        if (giftStepShowCircle.Count == 0 && giftStepShowRe.Count == 0)
        {
            parSerStepGift(AppConfig.defaultStepGift);
        }

        if (AdsHelper.Instance != null)
        {
            OpenAdShowtype = 4;
        }
        OpenAdShowtype = PlayerPrefs.GetInt("cf_open_ad_type", OpenAdShowtype);
        OpenAdShowat = PlayerPrefs.GetInt("cf_open_ad_showat", 2);
        OpenAdIsShowFirstOpen = PlayerPrefs.GetInt("cf_open_ad_show_firstopen", 0);
        OpenAdDelTimeOpen = PlayerPrefs.GetInt("cf_open_ad_deltime", 30);
        OpenadLvshow = PlayerPrefs.GetInt("cf_open_ad_level_show", 3);
        OpenAdTimeWaitShowFirst = PlayerPrefs.GetInt("cf_open_ad_wait_first", 60);

        cfLvStaticAds = PlayerPrefs.GetInt("cf_lv_static_ads", 0);
        typeMyopenAd = PlayerPrefs.GetInt("cf_type_myopenad", 0);

        stateShowAppLlovin = PlayerPrefs.GetInt("cf_state_show_applovin", 0);//vvv
        levelShowAppLovin = PlayerPrefs.GetInt("lv_show_apploovin", 10);//vvv
        tryloadApplovin = PlayerPrefs.GetInt("cf_try_reload_applovin", 1);
        loadOtherWhenLoadedApplovinButNotReady = PlayerPrefs.GetInt("cf_load_other_max_loaded", 0);

        stepFloorECPMBanner = PlayerPrefs.GetString("conf_banner_floor_ecpm", "");
        stepFloorECPMFull = PlayerPrefs.GetString("conf_full_floor_ecpm", "");
        stepFloorECPMGift = PlayerPrefs.GetString("conf_gift_floor_ecpm", "");

        specialType = PlayerPrefs.GetInt("cf_special_type", 0);
        specialData = PlayerPrefs.GetString("cf_special_data", "");
        parSerSpecialStep(specialData);
    }

    public void saveAllConfig()
    {
        PlayerPrefs.SetInt("cf_mask_ads_status", maskAdsStatus);
        PlayerPrefs.SetInt("cf_open_ad_type", OpenAdShowtype);
        PlayerPrefs.SetInt("cf_open_ad_showat", OpenAdShowat);
        PlayerPrefs.SetInt("cf_open_ad_show_firstopen", OpenAdIsShowFirstOpen);
        PlayerPrefs.SetInt("cf_open_ad_deltime", OpenAdDelTimeOpen);
        PlayerPrefs.SetInt("cf_open_ad_level_show", OpenadLvshow);
        PlayerPrefs.SetInt("cf_open_ad_wait_first", OpenAdTimeWaitShowFirst);

        PlayerPrefs.SetInt("cf_lv_static_ads", cfLvStaticAds);
        PlayerPrefs.SetInt("cf_type_myopenad", typeMyopenAd);

        PlayerPrefs.SetInt("android_build_ver_show_bn", verShowBanner);
        PlayerPrefs.SetInt("cf_state_show_applovin", stateShowAppLlovin);
        PlayerPrefs.SetInt("lv_show_apploovin", levelShowAppLovin);
        PlayerPrefs.SetInt("cf_try_reload_applovin", tryloadApplovin);
        PlayerPrefs.SetInt("cf_load_other_max_loaded", loadOtherWhenLoadedApplovinButNotReady);

        PlayerPrefs.SetString("conf_banner_floor_ecpm", stepFloorECPMBanner);
        PlayerPrefs.SetString("conf_full_floor_ecpm", stepFloorECPMFull);
        PlayerPrefs.SetString("conf_gift_floor_ecpm", stepFloorECPMGift);

        PlayerPrefs.SetInt("cf_special_type", specialType);
        PlayerPrefs.SetString("cf_special_data", specialData);

        saveBNConfig();
        saveFullConfig();
        saveGiftConfig();
    }

    public void saveBNConfig()
    {
        //banner
        PlayerPrefs.SetInt("cf_change_bn_sta", isChangeBNStatic);
        PlayerPrefs.SetString("cf_step_banner", stepShowBanner);
    }

    public void saveFullConfig()
    {
        //full
        PlayerPrefs.SetInt("cf_fullTotalOfday", fullTotalOfday);
        PlayerPrefs.SetInt("cf_fullLevelStart", fullLevelStart);
        PlayerPrefs.SetInt("cf_fullSessionStart", fullSessionStart);
        PlayerPrefs.SetInt("cf_full2LevelStart", full2LevelStart);
        PlayerPrefs.SetInt("cf_fullTimeStart", (fullTimeStart / 1000));
        PlayerPrefs.SetInt("cf_fulldefaultnumover", fullDefaultNumover);
        PlayerPrefs.SetInt("cf_full2numover", full2Numover);
        PlayerPrefs.SetInt("cf_fullShowPlaying", fullShowPlaying);
        PlayerPrefs.SetInt("cf_fullLoadAdsMobStatic", fullLoadAdsMobStatic);
        PlayerPrefs.SetString("cf_fullNumover", listIntervaltoString());
        PlayerPrefs.SetInt("cf_fullDeltatime", fullDeltatime);
        PlayerPrefs.SetInt("cf_full2Deltatime", full2Deltatime);
        PlayerPrefs.SetInt("cf_type_full_start", typeLoadStart);
        PlayerPrefs.SetInt("cf_load_maxlow", isLoadMaxLow);
        PlayerPrefs.SetString("cf_step_full", stepShowFull);
        PlayerPrefs.SetString("cf_step_full2", stepShowFull2);
        PlayerPrefs.SetString("cf_full_excluse_run", excluseFullrunning);
    }

    public void saveGiftConfig()
    {
        //gift
        PlayerPrefs.SetInt("cf_giftTotalOfday", giftTotalOfday);
        PlayerPrefs.SetInt("cf_giftDeltatime", giftDeltatime);
        PlayerPrefs.SetString("cf_step_gift", stepShowGift);
    }

    public void parSerStepBN(string step)
    {
        if (step == null || step.Length == 0)
        {
            return;
        }
        if (AppConfig.isOnlyDefault)
        {
            step = AppConfig.defaultStepBanner;
        }
#if ENABLE_ADS_ADMOB
        if (AdsHelper.Instance != null && changeTypeBanner())
        {
            step = "cir:0";
        }
#endif
        Debug.Log($"mysdk: isChangeBNStatic={isChangeBNStatic}, parSerStepBN={step}");
        string[] steptype = step.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        for (int it = 0; it < steptype.Length; it++)
        {
            if (steptype[it].StartsWith("cir:"))
            {
                string reSrep = steptype[it].Substring(4);
                string[] sl = reSrep.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                bnStepShowCircle.Clear();
                for (int i = 0; i < sl.Length; i++)
                {
                    try
                    {
                        int value = int.Parse(sl[i]);
                        bnStepShowCircle.Add(value);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else if (steptype[it].StartsWith("re:"))
            {
                string reSrep = steptype[it].Substring(3);
                string[] sl = reSrep.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                bnStepShowRe.Clear();
                for (int i = 0; i < sl.Length; i++)
                {
                    try
                    {
                        int value = int.Parse(sl[i]);
                        bnStepShowRe.Add(value);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }

    private bool changeTypeBanner()
    {
        if (isChangeBNStatic == 0)
        {
            return false;
        }
        int systemMemorySize = SystemInfo.systemMemorySize;
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        Debug.Log("mysdk: systemMemorySize=" + systemMemorySize);
        Debug.Log("mysdk: graphicsMemorySize=" + graphicsMemorySize);
#if UNITY_IOS || UNITY_IPHONE
        if (UnityEngine.iOS.Device.generation.ToString().Contains("iPhone"))
        {
            int something = (int)UnityEngine.iOS.Device.generation;
            if (something > 1 && something <= 32)
            {
                Debug.Log("mysdk: ios changeTypeBanner to static banner");
                return true;
            }
        }
#elif UNITY_ANDROID
        int sizemem = SystemInfo.systemMemorySize;
        if (sizemem < 1500)
        {
            Debug.Log("mysdk: Android changeTypeBanner to static banner");
            return true;
        }
#endif
        return false;
    }

    public void parSerStepNative(string step)
    {
        if (step == null || step.Length == 0)
        {
            return;
        }
        // Debug.Log("mysdk: parSerStepNative=" + step);
        string[] sl = step.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        nativeStepShow.Clear();
        for (int i = 0; i < sl.Length; i++)
        {
            int value;
            if (int.TryParse(sl[i], out value))
            {
                nativeStepShow.Add(value);
            }
        }
    }

    public string listIntervaltoString()
    {
        string re = "";
        for (int i = 0; i < listIntervalShow.Count; i++)
        {
            string sd = string.Format("{0},{1},{2}", listIntervalShow[i].startlevel, listIntervalShow[i].endLevel,
                listIntervalShow[i].deltal4Show);
            if (i == 0)
            {
                re += sd;
            }
            else
            {
                re += (";" + sd);
            }
        }

        return re;
    }

    public void parserIntervalnumoverfull(string data)
    {
        listIntervalShow.Clear();
        try
        {
            string[] arr = data.Split(new char[] { ';' });
            for (int i = 0; i < arr.Length; i++)
            {
                IntervalLevelShowfull inver = new IntervalLevelShowfull(arr[i]);
                if (inver.deltal4Show >= 0)
                {
                    listIntervalShow.Add(inver);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("mysdk: ex=" + ex.ToString());
        }
    }

    public void parSerExcluseFull(string excluse)
    {
        if (excluse == null || excluse.Length == 0)
        {
            return;
        }
        Debug.Log("mysdk: parSerExcluseFull=" + excluse);
        string[] steptype = excluse.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        fullExcluseShowRunning.Clear();
        for (int it = 0; it < steptype.Length; it++)
        {
            int adste;
            if (int.TryParse(steptype[it], out adste))
            {
                fullExcluseShowRunning.Add(adste);
            }
        }
    }

    public void parSerStepFull(string step)
    {
        if (step == null || step.Length == 0)
        {
            return;
        }
        if (AppConfig.isOnlyDefault)
        {
            step = AppConfig.defaultStepFull;
        }
        Debug.Log("mysdk: parSerStepFull=" + step);
        string[] steptype = step.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        for (int it = 0; it < steptype.Length; it++)
        {
            if (steptype[it].StartsWith("cir:"))
            {
                string reSrep = steptype[it].Substring(4);
                string[] sl = reSrep.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                fullStepShowCircle.Clear();
                for (int i = 0; i < sl.Length; i++)
                {
                    try
                    {
                        int value = int.Parse(sl[i]);
                        fullStepShowCircle.Add(value);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else if (steptype[it].StartsWith("re:"))
            {
                string reSrep = steptype[it].Substring(3);
                string[] sl = reSrep.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                fullStepShowRe.Clear();
                for (int i = 0; i < sl.Length; i++)
                {
                    try
                    {
                        int value = int.Parse(sl[i]);
                        fullStepShowRe.Add(value);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }

    public void parSerStepFull2(string step)
    {
        if (step == null || step.Length == 0)
        {
            return;
        }
        Debug.Log("mysdk: parSerStepFull2=" + step);
        string[] steplist = step.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        full2StepShowCircle.Clear();
        for (int i = 0; i < steplist.Length; i++)
        {
            try
            {
                int value = int.Parse(steplist[i]);
                full2StepShowCircle.Add(value);
            }
            catch (Exception)
            {

            }
        }
    }

    public void parSerStepGift(string step)
    {
        if (step == null || step.Length == 0)
        {
            return;
        }
        if (AppConfig.isOnlyDefault)
        {
            step = AppConfig.defaultStepGift;
        }
        Debug.Log("mysdk: parSerStepGift=" + step);
        string[] steptype = step.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        for (int it = 0; it < steptype.Length; it++)
        {
            if (steptype[it].StartsWith("cir:"))
            {
                string reSrep = steptype[it].Substring(4);
                string[] sl = reSrep.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                giftStepShowCircle.Clear();
                for (int i = 0; i < sl.Length; i++)
                {
                    try
                    {
                        int value = int.Parse(sl[i]);
                        giftStepShowCircle.Add(value);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else if (steptype[it].StartsWith("re:"))
            {
                string reSrep = steptype[it].Substring(3);
                string[] sl = reSrep.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                giftStepShowRe.Clear();
                for (int i = 0; i < sl.Length; i++)
                {
                    try
                    {
                        int value = int.Parse(sl[i]);
                        giftStepShowRe.Add(value);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }

    public void parSerSpecialStep(string data)
    {
        listSpecialShow.Clear();
        var liststep = (List<object>)JsonDecoder.DecodeText(specialData);
        if (liststep != null && liststep.Count > 0)
        {
            foreach (object ob in liststep)
            {
                IDictionary<string, object> dicob = (IDictionary<string, object>)ob;
                SpecialConditionShow spec = new SpecialConditionShow(dicob);
                listSpecialShow.Add(spec);
            }
        }
    }

    public void parSerSpecial(IDictionary<string, object> dicdata)
    {
        listSpecialShow.Clear();
        if (dicdata == null || dicdata.Count == 0)
        {
            return;
        }
        specialType = 0;
        if (dicdata.ContainsKey("type"))
        {
            specialType = Convert.ToInt32(dicdata["type"]);
        }
        if (dicdata.ContainsKey("data"))
        {
            List<object> liststep = (List<object>)dicdata["data"];
            if (liststep != null && liststep.Count > 0)
            {
                specialData = "[";
                bool isStart = true;
                foreach (object ob in liststep)
                {
                    IDictionary<string, object> dicob = (IDictionary<string, object>)ob;
                    SpecialConditionShow spec = new SpecialConditionShow(dicob);
                    listSpecialShow.Add(spec);
                    if (!isStart)
                    {
                        specialData += ",";
                    }
                    specialData += spec.toJsonOb();

                    isStart = false;
                }
                specialData += "]";
            }
        }
    }

}

//-------------------------------------------------
public class IntervalLevelShowfull
{
    public int startlevel;
    public int endLevel;
    public int deltal4Show;

    public IntervalLevelShowfull(string data)
    {
        fromStringData(data);
    }

    public void fromStringData(string data)
    {
        deltal4Show = -1;
        try
        {
            string[] sarr = data.Split(new char[] { ',' });
            if (sarr.Length == 3)
            {
                startlevel = int.Parse(sarr[0]);
                endLevel = int.Parse(sarr[1]);
                deltal4Show = int.Parse(sarr[2]);
                if (startlevel >= endLevel)
                {
                    deltal4Show = -1;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("mysdk: fromStringData ex=" + ex.ToString());
        }
    }
}
//------------------------------------------------
public class SpecialConditionShow
{
    public int startCon;//con session, timeplay
    public int endCon;//con session, timeplay

    public int deltal4Show = 0;
    public string stepFull = "";
    public string stepGift = "";

    public SpecialConditionShow(IDictionary<string, object> dicdata)
    {
        fromStringData(dicdata);
    }

    public void fromStringData(IDictionary<string, object> dicdata)
    {
        deltal4Show = 0;
        stepFull = "";
        stepGift = "";
        try
        {
            if (dicdata.ContainsKey("start"))
            {
                startCon = Convert.ToInt32(dicdata["start"]);
            }
            if (dicdata.ContainsKey("end"))
            {
                endCon = Convert.ToInt32(dicdata["end"]);
            }
            if (dicdata.ContainsKey("delta"))
            {
                deltal4Show = Convert.ToInt32(dicdata["delta"]);
            }
            if (dicdata.ContainsKey("full"))
            {
                stepFull = (string)dicdata["full"];
            }
            if (dicdata.ContainsKey("gift"))
            {
                stepGift = (string)dicdata["gift"];
            }
        }
        catch (Exception ex)
        {
            Debug.Log("mysdk: fromStringData ex=" + ex.ToString());
        }
    }

    public string toJsonOb()
    {
        string sjson = "{";
        sjson += $"\"start\":{startCon}";
        sjson += $",\"end\":{endCon}";
        sjson += $",\"delta\":{deltal4Show}";
        if (stepFull != null && stepFull.Length > 3)
        {
            sjson += $",\"full\":\"{stepFull}\"";
        }
        if (stepFull != null && stepFull.Length > 3)
        {
            sjson += $",\"gift\":\"{stepGift}\"";
        }
        sjson += "}";

        return sjson;
    }
}
