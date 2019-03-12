using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eUnloadBundleType
{
    NONE = 0,
    LOADED = 1,
    LEVELLOADED = 2,
    NEVER = 3,
}


struct ABRequestUnit
{
    public AssetBundleRequest assetBundleRequest;
    public List<AssetBundleItem> lstDependentItems;
    public ResourceInfo resInfo;
}

public class AssetItem
{
    public string name = string.Empty;
    public string type = "NONE";
    public uint bundleId = 0;
    //public bool isBig = true;
    //public float floatParam = 0;
    public List<uint> dependencies = null;
}

public class AssetBundleItem
{
    public uint uID = 0;
    public int nChapter = 0; //根据章节下载的章节编号
    public string strName = string.Empty;
    //public string srcMD5 = string.Empty;
    //public string decMD5 = string.Empty;
    //public bool isInApp = false;
    public long nSize = 0;
    public List<AssetItem> lstAssets = null;
    public AssetBundle assetBundle = null;
    public eUnloadBundleType eUnloadType = eUnloadBundleType.NONE;
    public int nDependenciedCount = 0;
}

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    bool m_bActive = false;

    Dictionary<string, AssetItem> m_dicAllAssets = new Dictionary<string, AssetItem>();

    public void Init()
    {
        m_bActive = true;
        GameRoot.Instance.StartCoroutine(CoLoadProcess());
    }

    public void Term()
    {
        m_bActive = false;
        GameRoot.Instance.StopCoroutine(CoLoadProcess());
    }

    public UnityEngine.Object LoadSync(string strPath)
    {
        return null;
    }

    private IEnumerator CoLoadProcess()
    {
        while(m_bActive)
        {
            yield return null;
        }
    }

    public AssetBundleItem GetAssetBundleItem(string strResourcePath)
    {
        return null;
    }

    public List<uint> GetBundleAndDependencies(string path)
    {
        List<uint> dependence = new List<uint>();

        AssetItem asset = null;
        if (m_dicAllAssets.TryGetValue(path, out asset))
        {
            if (asset.dependencies != null && asset.dependencies.Count > 0)
            {
                dependence.AddRange(asset.dependencies);
            }

            dependence.Add(asset.bundleId);
        }

        return dependence;
    }
}
