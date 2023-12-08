using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace ads.myeditor
{
    public class BigoAdsUtil
    {
        public static void addRemoveAds(string stateAds)
        {
            try
            {
                string[] arrsss = stateAds.Split(';');
                int isAdd = 0;
                int typeAds = 0;
                string verAndroid = "2.9.0.0";
                string verios = "2.2.0";
                if (arrsss.Length == 3)
                {
                    isAdd = int.Parse(arrsss[0]);
                    typeAds = int.Parse(arrsss[1]);
#if UNITY_ANDROID
                    verAndroid = arrsss[2];
#else
                    verios = arrsss[2];
#endif
                }
                if (typeAds == 6)
                {
                    string pathdir = Application.dataPath + "/MaxSdk/Mediation/BigoAds/Editor";
                    if (isAdd == 1)
                    {
                        string pathmax = Application.dataPath + "/MaxSdk/AppLovin.meta";
                        if (File.Exists(pathmax))
                        {
                            Directory.CreateDirectory(pathdir);
                            string pathfile = pathdir + "/Dependencies.xml";
                            List<string> allline = new List<string>();
                            allline.Add("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
                            allline.Add("<dependencies>");
                            allline.Add("    <androidPackages>");
                            allline.Add($"        <androidPackage spec = \"com.bigossp:max-mediation:{verAndroid}\"/>");
                            allline.Add("            <repositories>");
                            allline.Add("                <repository>https://repo1.maven.org/maven2/</repository>");
                            allline.Add("            </repositories>");
                            allline.Add("    </androidPackages>");
                            allline.Add("    <iosPods>");
                            allline.Add($"        <iosPod name = \"bigo-ads-max-adapter\" version = \"{verios}\"/>");
                            allline.Add("    </iosPods>");
                            allline.Add("</dependencies>");

                            System.IO.File.WriteAllLines(pathfile, allline);
                        }
                    }
                    else
                    {
                        string pathfile = pathdir + "/Dependencies.xml";
                        if (File.Exists(pathfile))
                        {
                            File.Delete(pathfile);
                            pathfile = pathdir + "/Dependencies.xml.meta";
                            File.Delete(pathfile);
                        }
                    }
                }
                else if (typeAds == 3)
                {
                    string pathdir = Application.dataPath + "/IronSource/Editor";
                    if (isAdd == 1)
                    {
                        string pathmax = Application.dataPath + "/IronSource/Plugins.meta";
                        if (File.Exists(pathmax))
                        {
                            Directory.CreateDirectory(pathdir);
                            string pathfile = pathdir + "/ISBigoAdsAdapterDependencies.xml";
                            List<string> allline = new List<string>();
                            allline.Add("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
                            allline.Add("<dependencies>");
                            allline.Add("    <androidPackages>");
                            allline.Add($"        <androidPackage spec = \"com.bigossp:ironsource-mediation:{verAndroid}\"/>");
                            allline.Add("            <repositories>");
                            allline.Add("                <repository>https://repo1.maven.org/maven2/</repository>");
                            allline.Add("            </repositories>");
                            allline.Add("    </androidPackages>");
                            allline.Add("    <iosPods>");
                            allline.Add($"        <iosPod name = \"bigo-ads-ironsource-adapter\" version = \"{verios}\"/>");
                            allline.Add("    </iosPods>");
                            allline.Add("</dependencies>");

                            System.IO.File.WriteAllLines(pathfile, allline);
                        }
                    }
                    else
                    {
                        string pathfile = pathdir + "/ISBigoAdsAdapterDependencies.xml";
                        if (File.Exists(pathfile))
                        {
                            File.Delete(pathfile);
                            pathfile = pathdir + "/ISBigoAdsAdapterDependencies.xml.meta";
                            File.Delete(pathfile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("mysdk: addRemoveAds bigo ex=" + ex.ToString());
            }
        }
    }
}