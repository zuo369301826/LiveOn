using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class AssetsManager : Singleton<AssetsManager> {

    private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
    //private List<AssetBundle> bundles= new List<AssetBundle>();
    private LogInformationList logInformationList= new LogInformationList();
    private UnityEngine.Object obj = null;
    private GameObject go;
    private AssetBundle ab = null;
    List<string> assetBundleNameList = new List<string>();
    // Use this for initialization
    public void Init()
    {
        //读取配置文件
        ReadLogTxt();     
    }

    //AssetsBundle加载资源
    public Object LoadResource(string assetPath)
    {     
        assetBundleNameList = logInformationList.GetAssetBundleNameList(assetPath);
        AssetBundle temp = null;
        string datapath;
#if UNITY_STANDALONE_WIN
        datapath = "Assets/StreamingAssets/";
#endif
#if UNITY_ANDROID
        datapath = Application.dataPath + "!assets/";
#endif       

        for (int i = 0; i < assetBundleNameList.Count; i++)
        {
            if (!bundles.ContainsKey(assetBundleNameList[i]))
            {
                temp = AssetBundle.LoadFromFile(datapath + assetBundleNameList[i]);
                bundles.Add(temp.name, temp);
            }
        }
        bundles.TryGetValue(logInformationList.getLogInformationByPath(assetPath).GetAssetBundleName(),out ab);
        if (ab != null)
        {
            obj = ab.LoadAsset(assetPath);
            ab = null;
        }
        return obj;
    }

    //释放资源
    public void ReleaseResource() {
        obj = null;
        go = null;
        //for (int i = 0; i < bundles.Values; i++) {
           // bundles.Values[i]
            //bundles[i].Unload(false);
        //}
        Resources.UnloadUnusedAssets();
    }

    //读取配置文件
    private void ReadLogTxt() {
#if UNITY_STANDALONE_WIN
        string filePath = "Assets/StreamingAssets/AssetBundleLog.txt";
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        StreamReader streamReader = File.OpenText(filePath);
        string line;
        streamReader.ReadLine();
        while ((line = streamReader.ReadLine()) != null)
        {
            string[] logInformation = line.Split(',');
            int id = int.Parse(logInformation[0]);
            string assetName = logInformation[1];
            string path = logInformation[2];
            string assetBundleName = logInformation[6];                     
            List<int> dependencyId = new List<int>();
            for (int i = 7; i < logInformation.Length - 1; i++)
            {
                dependencyId.Add(int.Parse(logInformation[i]));
            }

            logInformationList.AddLogInformation(id, assetName, path, assetBundleName, dependencyId);
        }

        streamReader.Close();
        fileStream.Close();
#endif
#if UNITY_ANDROID
        string url = Application.streamingAssetsPath + "/" + "AssetBundleLog.txt";
        WWW www = new WWW(url);
        while (!www.isDone) { }
        string[] line = www.text.Split('\n');
        for (int i = 1; i < line.Length - 1; i++)
        {
            string[] logInformation = line[i].Split(',');
            int id = int.Parse(logInformation[0]);
            string assetName = logInformation[1];
            string path = logInformation[2];
            string assetBundleName = logInformation[6];
            List<int> dependencyId = new List<int>();
            for (int j = 7; j < logInformation.Length - 1; j++)
            {
                dependencyId.Add(int.Parse(logInformation[j]));
            }
            logInformationList.AddLogInformation(id, assetName, path, assetBundleName, dependencyId);
        }
#endif
        }

    }
