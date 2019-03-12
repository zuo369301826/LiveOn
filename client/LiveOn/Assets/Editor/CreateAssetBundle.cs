using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CreateAssetBundle {

    private static DirectoryInfo dire = new DirectoryInfo("Assets");
    private static string sign = "";
    private static string[] expand = new string[] { ".prefab", ".fbx", ".png", ".ogg", ".obj", ".asset", ".mp3", ".shader",
                                                    ".wav", ".tga", ".jpg", ".physicMaterial", ".psd", "physicsMaterial2D",
                                                    ".anim", ".controller", ".nav", ".mat", ".ttf", ".mask",};
    private static ResourceList list;
    private static DependencyGraph graph;

    [MenuItem("AssetBundle/BuildAssetBundles")]
    public static void BuildAssetBundles() {

        list = new ResourceList();
        graph = new DependencyGraph();

        List<Node> resourceList = list.GetResourceList();
        //Node root = graph.GetRootNode();

        FindDirectories(dire);      //从指定目录下查找指定类型的资源文件，并封装成Node类型对象，添加进资源列表和资源依赖关系图中

        //for (int i = 0; i < list.GetResourceList().Count; i++)
        //{
        //    Debug.Log(list.GetResourceList()[i].GetId() + " " + list.GetResourceList()[i].GetPath());
        //}

        //查找所有资源的依赖关系
        for (int i = 0; i < resourceList.Count; i++)
        {
            string[] fileNames = AssetDatabase.GetDependencies(resourceList[i].GetPath(), false);

            for (int j = 0; j < fileNames.LongLength; j++)
            {
                if (fileNames[j].EndsWith(".cs"))
                    continue;
                //Debug.Log(resourceList[i].GetId() + " 依赖于 " + list.FindResourceByPath(fileNames[j]).GetId());
                graph.AddDependent(resourceList[i], list.FindResourceByPath(fileNames[j]));                 //将依赖关系添加到依赖关系图中
            }
        }

        AssetBundleBuildMap oldMap = LoadLog();                               //旧build map 由日志文件信息生成
        AssetBundleBuildMap map = graph.CreateAssetBundleBuildMap();          //新build map 由资源依赖关系图生成
        map.UpdateAssetBundleName();    
        list.RecoverIsPass();    

        //-------------------------------------------
        //差量更新  变更资源判断，只更新变动的 asset bundle.

        List<MyAssetBundleBuild> oldMyAssetBundleBuildMap = oldMap.GetAssetBundleBuildMap();
        List<MyAssetBundleBuild> myAssetBundleBuildMap = map.GetAssetBundleBuildMap();              

        for (int i = 0; i < oldMyAssetBundleBuildMap.Count; i++)
        {
            for (int j = 0; j < myAssetBundleBuildMap.Count; j++)
            {
                if (myAssetBundleBuildMap[j].GetAssetBunldeName().Equals(oldMyAssetBundleBuildMap[i].GetAssetBunldeName()))
                {
                    map.RemoveAssetBundle(myAssetBundleBuildMap[j]);
                    j--;
                    break;
                }
            }
        }

        AssetBundleBuild[] buildMap = new AssetBundleBuild[myAssetBundleBuildMap.Count];

        for (int i = 0; i < myAssetBundleBuildMap.Count; i++)
        {
            if (myAssetBundleBuildMap[i].GetAssetBunldeName().Equals("root"))
            {
                continue;
            }
            List<Node> assets = myAssetBundleBuildMap[i].GetAssetList();

            buildMap[i].assetBundleName = myAssetBundleBuildMap[i].GetAssetBunldeName();

            string[] assetNames = new string[assets.Count];

            for (int j = 0; j < assets.Count; j++)
            {
                assetNames[j] = assets[j].GetPath();
            }

            buildMap[i].assetNames = assetNames;
        }

        //Debug.Log(buildMap.Length);
        //for (int i = 0; i < buildMap.Length; i++)
        //{
        //    Debug.Log(buildMap[i].assetBundleName);
        //    for (int j = 0; j < buildMap[i].assetNames.Length; j++)
        //        Debug.Log(buildMap[i].assetNames[j]);
        //}

#if UNITY_STANDALONE_WIN
        //打包
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        //BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
        //创建日志文件
#endif
#if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
#endif
        CreateLog();
    }

    //查找指定类型的所有资源文件，并添加到资源列表和资源依赖关系图中
    private static void FindDirectories(DirectoryInfo dire) {

        DirectoryInfo[] dires = dire.GetDirectories("*", SearchOption.TopDirectoryOnly);
        FileInfo[] files = dire.GetFiles("*", SearchOption.TopDirectoryOnly);
        if (dires != null) 
        {
            for (int i = 0; i < dires.Length; i++)
            {
                if (dires[i].Name.StartsWith(sign))
                {
                    FindResources(dires[i]);
                }
                else
                {
                    FindDirectories(dires[i]);
                }
            }
        }
        if (files != null) {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.StartsWith(sign))
                {
                    IsUsedResource(files[i]);
                }
            }
        }       
    }

    private static void FindResources(DirectoryInfo directory) {

        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++) {
            IsUsedResource(files[i]);
        }
    }

    private static void IsUsedResource(FileInfo file) {

        for (int j = 0; j < expand.Length; j++)
        {
            if (file.Name.ToLower().EndsWith(expand[j].ToLower()))
            {
                int id = list.GetResourceList().Count;
                string assetPath = file.Name;
                DirectoryInfo dire = file.Directory;
                while (!dire.Name.Equals("LiveOn"))
                {
                    assetPath = dire.Name + '/' + assetPath;
                    dire = dire.Parent;
                }
                //assetPath = "Assets/" + assetPath;
                string md5 = GetFileMd5(assetPath);
                string metaPath = assetPath + ".meta";
                string metaMd5 = GetFileMd5(metaPath);

                Node node = new Node(id, assetPath, md5, metaPath, metaMd5);
                list.AddResource(node);
                graph.AddResource(node);
                break;
            }
        }
    }

    //获取指定文件的md5码
    private static string GetFileMd5(string path)
    {
        FileStream file = new FileStream(path, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);
        file.Close();

        StringBuilder Ac = new StringBuilder();

        for (int i = 0; i < retVal.Length; i++)
        {
            Ac.Append(retVal[i].ToString("x2"));
        }

        return Ac.ToString();

    }

    //读取日志信息
    private static AssetBundleBuildMap LoadLog() {

        FileStream fileStream = new FileStream("Assets/StreamingAssets/AssetBundleLog.txt", FileMode.OpenOrCreate, FileAccess.Read);
        StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);

        AssetBundleBuildMap map = new AssetBundleBuildMap();

        streamReader.ReadLine();        //第一行表头不需要读取

        string line;
        while ((line = streamReader.ReadLine()) != null)
        {

            string[] logInformation = line.Split(',');
            int id = int.Parse(logInformation[0]);
            //string name = logInformation[1];
            string path = logInformation[2];
            string md5 = logInformation[3];
            string metaName = logInformation[4];
            string metaMd5 = logInformation[5];
            string assetBundleName = logInformation[6];

            Node node = new Node(id, path, md5, metaName, metaMd5, assetBundleName);

            map.AddAsset(node, assetBundleName);
           
        }

        streamReader.Close();
        fileStream.Close();

        return map;
    }

    //创建日志信息
    private static void CreateLog() {

        FileStream fileStream = new FileStream("Assets/StreamingAssets/AssetBundleLog.txt", FileMode.Create, FileAccess.Write);
        StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

        string title = "id,name,path,md5,meta path,meta md5,asset bundle name,dependency asset id";
      
        streamWriter.WriteLine(title);

        List<Node> resourceList = list.GetResourceList();

        for (int i = 0; i < resourceList.Count; i++)
        {           
            string log = "";
            log += resourceList[i].GetId() + ",";
            log += resourceList[i].GetName() + ",";
            log += resourceList[i].GetPath() + ",";
            log += resourceList[i].GetMd5() + ",";
            log += resourceList[i].GetMetaName() + ",";
            log += resourceList[i].GetMetaMd5() + ",";
            log += resourceList[i].GetAssetBundleName() + ",";

            List<int> idList = new List<int>();
            graph.GetDependencyAssetsId(resourceList[i], idList);
            list.RecoverIsPass();

            for (int j = 1; j < idList.Count; j++)
            {
                log += idList[j] + ",";
            }

            streamWriter.WriteLine(log);
        }
        
        streamWriter.Close();
        fileStream.Close();
    }
}