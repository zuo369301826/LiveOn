using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

//asset bundle 打包的信息
public class MyAssetBundleBuild {

    private string assetBundleName;                 //asset bundle名称         
    private List<Node> assets;                      //asset bundle包内的所有资源
    private double size;                             //asset bundle大小 

    //构造方法
    public MyAssetBundleBuild()
    {
        assets = new List<Node>();
        size = 0f;
    }

    //设置asset bundle名称
    public void SetAssetBundleName(string assetBundleName) {
        this.assetBundleName = assetBundleName;
    }

    //得到资源列表
    public List<Node> GetAssetList()
    {
        return assets;
    }

    //得到asset bundle名称
    public string GetAssetBunldeName() {
        return assetBundleName;
    }

    public double GetSize() {
        return size;
    }

    //添加一个资源
    public void AddAsset(Node node) {

        for (int i = 0; i < assets.Count; i++)
        {
            if (node.GetId() == assets[i].GetId())
                return;
        }

        FileInfo file = new FileInfo(node.GetPath());

        size += System.Math.Ceiling(file.Length / 1024.0f);

        assets.Add(node);
    }
}

//所有 asset bundle 打包的信息
public class AssetBundleBuildMap {

    private List<MyAssetBundleBuild> buildMap;      // asset bundle包列表

    //构造方法
    public AssetBundleBuildMap() {

        buildMap = new List<MyAssetBundleBuild>();

    }       

    //得到asset bundle包列表
    public List<MyAssetBundleBuild> GetAssetBundleBuildMap() {

        return buildMap;

    }

    ////添加一个独立资源到指定的asset bundle包下
    //public bool AddAloneAsset(Node node, string assetBundleName) {

    //    for (int i = 0; i < buildMap.Count; i++)
    //    {
    //        if (assetBundleName.Equals(buildMap[i].GetAssetBunldeName()))
    //        {
    //            if (buildMap[i].GetAssetList().Count > 5 || buildMap[i].GetSize() > 5)              //如果ab包内资源数大于5或大于5M，不再放资源
    //            {
    //                return false;
    //            }
    //            buildMap[i].AddAsset(node);
    //            return true;
    //        }
    //    }

    //    MyAssetBundleBuild assetBuildMap = new MyAssetBundleBuild();
    //    assetBuildMap.SetAssetBundleName(assetBundleName);
    //    assetBuildMap.AddAsset(node);
    //    buildMap.Add(assetBuildMap);
    //    return true;
    //}

    //添加一个资源到指定的asset bundle包下
    public void AddAsset(Node node, string assetBundleName)
    {

        for (int i = 0; i < buildMap.Count; i++)
        {
            if (assetBundleName.Equals(buildMap[i].GetAssetBunldeName()))
            {
                buildMap[i].AddAsset(node);
                return;
            }
        }

        MyAssetBundleBuild assetBuildMap = new MyAssetBundleBuild();
        assetBuildMap.SetAssetBundleName(assetBundleName);
        assetBuildMap.AddAsset(node);
        buildMap.Add(assetBuildMap);
    }

    //删除指定的asset bundle包
    public void RemoveAssetBundle(MyAssetBundleBuild build) {
        buildMap.Remove(build);
    }

    public void UpdateAssetBundleName() {
       
        for (int i = 0; i < buildMap.Count; i++)
        {
            string assetBundleName = "";
            for (int j = 0; j < buildMap[i].GetAssetList().Count; j++)
            {
                assetBundleName += buildMap[i].GetAssetList()[j].GetMd5();
            }

            string newMd5 = GetMd5(assetBundleName);
            buildMap[i].SetAssetBundleName(newMd5);

            for (int j = 0; j < buildMap[i].GetAssetList().Count; j++)
            {
                buildMap[i].GetAssetList()[j].SetAssetBundleName(newMd5);
            }
        }
    }

    private string GetMd5(string input) {

        if (input == null)
            return null;

        System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();

        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++) 
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
}