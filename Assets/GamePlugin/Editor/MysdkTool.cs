﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class MysdkTool : MonoBehaviour
{
    [MenuItem("Tools/Mysdk/BackupFile")]
    static void BackupFile()
    {
        string path = "Assets";
        string GameName = SettingBuildAndroid.getMemSetting("key_mem_gamename", "");

        string passet = Application.dataPath;
        string ppro = passet.Replace("/Assets", "");
        string pathRes = ppro + $"/Mem{GameName}/Res";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
					string pra = path.Replace("Assets", "");
					string pfrom = passet + "/" + pra;
					string pto = pathRes + pra;
					createRootDir(pto);
					SettingBuildAndroid.coppyFileNormal(pfrom, pto);
                }
                else if (Directory.Exists(path) && !path.Contains("Assets"))
                {
					string pra = path.Replace("Assets", "");
					string pfrom = passet + "/" + pra;
					string pto = pathRes + pra;
					SettingBuildAndroid.coppyFolder(pfrom, pto, 1);
                }
            }
        }
        SettingBuildAndroid.backupGamePlugin(GameName);
    }

    [MenuItem("Tools/ClearAllData")]
    static void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        try
        {
            string pathgame = Application.persistentDataPath;
            delFolder(pathgame);
        }
        catch (Exception ex)
        {

        }
    }

    static void createRootDir(string path)
    {
        try
        {
            if (!string.IsNullOrEmpty(path))
            {
                string pdir = Path.GetDirectoryName(path);
                if (!Directory.Exists(pdir))
                {
                    Directory.CreateDirectory(pdir);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("mysdk: createRootDir ex=" + ex.ToString());
        }

    }

    static void delFolder(string path)
    {
        if (Directory.Exists(path))
        {
            string[] listDirs = Directory.GetDirectories(path);
            foreach (string subpath in listDirs)
            {
                delFolder(subpath);
            }
            string[] listFiles = Directory.GetFiles(path);
            foreach (string subpath in listFiles)
            {
                File.Delete(subpath);
            }
            Directory.Delete(path);
        }
    }
}
