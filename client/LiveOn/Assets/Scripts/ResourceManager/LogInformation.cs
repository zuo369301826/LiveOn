using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogInformation {

    private int id;
    private string assetName;
    private string path;
    private string assetBundleName;
    private List<int> dependencyId;

    //构造方法
    public LogInformation(int id, string assetName, string path, string assetBundleName, List<int> dependencyId) {

        this.id = id;
        this.assetName = assetName;
        this.path = path;
        this.assetBundleName = assetBundleName;
        this.dependencyId = dependencyId;

    }

    //得到资源编号
    public int GetId() {

        return id;

    }

    //得到资源名
    public string GetAssetName() {

        return assetName;

    }

    //得到资源路径
    public string GetPath() {

        return path;

    }

    //得到所在asset bundle包名
    public string GetAssetBundleName() {

        return assetBundleName;

    }

    //得到直接依赖资源的资源编号列表
    public List<int> GetDependencyId() {

        return dependencyId;

    }

}

public class LogInformationList {

    private List<string> assetBundleNameList;           //所有 asset bundle 包名

    private List<LogInformation> logInformationList;    

    //构造方法
    public LogInformationList() {

        logInformationList = new List<LogInformation>();
        assetBundleNameList = new List<string>();

    }

    //增加一条配置文件信息
    public void AddLogInformation(int id, string assetName, string path, string assetBundleName, List<int> dependencyId) {

        LogInformation logInformation = new LogInformation(id, assetName, path, assetBundleName, dependencyId);
        logInformationList.Add(logInformation);

    }

    //得到指定资源文件的所有需要加载的 asset bundle 包名
    public List<string> GetAssetBundleNameList(string assetPath) {

        assetBundleNameList = new List<string>();

        LogInformation log = getLogInformationByPath(assetPath);
        assetBundleNameList.Add(log.GetAssetBundleName());
        for(int i = 0; i < log.GetDependencyId().Count; i++)
        {
            assetBundleNameList.Add(getLogInformationByAssetId(log.GetDependencyId()[i]).GetAssetBundleName());
        }

        //findDependencyAsset(assetName);
        return assetBundleNameList;

    }

    //查找指定资源的所有依赖资源
    private void findDependencyAsset(string assetName) {

        LogInformation log = getLogInformationByAssetName(assetName);
        AddAssetBundleName(log.GetAssetBundleName());

        List<int> logChildren = log.GetDependencyId();
        for (int i = 0; i < logChildren.Count; i++) 
        {
            LogInformation logchild = getLogInformationByAssetId(logChildren[i]);
            findDependencyAsset(logchild.GetAssetName());
        }

    }

    //得到所有 asset bundle 包名
    private List<string> GetAssetBundleNameList() {

        return assetBundleNameList;

    }

    //增加一个 asset bundle 名
    private void AddAssetBundleName(string AssetBundleName) {

        for (int i = 0; i < assetBundleNameList.Count; i++)
        {
            if (assetBundleNameList[i].Equals(AssetBundleName))
            {
                return;
            }
        }

        assetBundleNameList.Add(AssetBundleName);


    }

    //通过资源名找到对应的配置文件中的一行信息
    public LogInformation getLogInformationByAssetName(string assetName) {

        for (int i = 0; i < logInformationList.Count; i++)
        {
            if (assetName.Equals(logInformationList[i].GetAssetName()))
            {
                return logInformationList[i];
            }

        }

        return null;

    }

    //通过资源编号找到对应的配置文件中的一行信息
    private LogInformation getLogInformationByAssetId(int id){

        for (int i = 0; i < logInformationList.Count; i++)
        {
            if (id == logInformationList[i].GetId())
            {
                return logInformationList[i];
            }

        }

        return null;

    }

    //通过资源路径找到对应的配置文件中的一行信息
    public LogInformation getLogInformationByPath(string path) {

        for (int i = 0; i < logInformationList.Count; i++)
        {
            if (path.Equals(logInformationList[i].GetPath()))
            {
                return logInformationList[i];
            }

        }

        return null;

    }

}