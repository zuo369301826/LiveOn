using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum eReqPrior
{
    REQ_PRIOR_AT_ONCE,
    REQ_PRIOR_HIGHEST,
    REQ_PRIOR_HIGH,
    REQ_PRIOR_NORMAL,
    REQ_PRIOR_LOW,
    REQ_PRIOR_LOWEST,
};

public enum eResStatus
{
    WAITING,
    LOADING,
    LOADED,
    READY,
    IGNORED,
};

public class ResourceInfo
{
    public ResLoadCallback fnCallBack;

    //public static bool m_sLoadFromAssetBundle = true;

    private UnityEngine.Object m_Object = null;

    private eResStatus m_eStatus = eResStatus.WAITING;

    private eReqPrior m_ePrior;

    private string m_strPath;

    private bool m_bClone = false;

    private RequestInfo m_Request;

    public string Path
    {
        get { return m_strPath; }
        set { m_strPath = value; }
    }

    public eReqPrior Prior
    {
        get { return m_ePrior; }
        set { m_ePrior = value; }
    }

    public RequestInfo Request
    {
        get { return m_Request; }
        set { m_Request = value; }
    }

    public UnityEngine.Object Obj
    {
        get { return m_Object; }
    }

    public bool Available()
    {
        return m_eStatus == eResStatus.LOADED || m_eStatus == eResStatus.READY;
    }

    private void OnLoaded(Object obj)
    {
        m_Object = obj;

        m_eStatus = eResStatus.LOADED;

        if (m_Request != null)
        {
            if (m_Request.fnRequestCallback != null)
            {
                m_Request.fnRequestCallback(this);
            }
        }
    }

    public void Load()
    {
        UnityEngine.Object srcObj = null;
#if AssetBundle
        srcObj = AssetsManager.instance.LoadResource(m_strPath.Replace("\\", "/"));
#endif

#if AssetDatabase
        srcObj = UnityEditor.AssetDatabase.LoadAssetAtPath(m_strPath.Replace("\\", "/"), typeof(UnityEngine.Object));
#endif
        OnLoaded(srcObj);
    }

    public void LoadAsync()
    {
        UnityEngine.Object srcObj = null;


#if AssetBundle
        srcObj = AssetsManager.instance.LoadResource(m_strPath.Replace("\\", "/"));
#endif

#if AssetDatabase
        srcObj = UnityEditor.AssetDatabase.LoadAssetAtPath(m_strPath.Replace("\\", "/"), typeof(UnityEngine.Object));
#endif
        OnLoaded(srcObj);
    }

    public void Reset()
    {
        m_Object = null;
        m_Request = null;
        m_strPath = string.Empty;
    }
}

public delegate void ResLoadCallback(ResourceInfo resource);
public delegate void RequestProcessCallback(ResourceInfo res);

public class RequestInfo
{
    public enum eReqType
    {
        REQ_LOAD,
        REQ_FREE,
    };
    public ResourceInfo res;
    public eReqType eType;
    public RequestProcessCallback fnRequestCallback;

    //public bool team;
    public List<uint> lstDependentItems;
}

public struct ObjectUseInfo
{
    public uint nUseCount;
    public UnityEngine.Object m_Object;
}

public class ResourceManager : Singleton<ResourceManager>
{
    private bool m_bActive = false;
    private long m_nLoadBeginTime = 0;

    private List<RequestInfo> m_ListRequest = new List<RequestInfo>(256);

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

    public void Tick()
    {


    }

    // 同步资源加载 
    public ResourceInfo LoadResource(string strPath)
    {
        ResourceInfo res = new ResourceInfo();
        res.Path = strPath;
        res.Load();
        return res;
    }

    // 异步资源加载
    public ResourceInfo LoadResource(string strPath, ResLoadCallback callback)
    {
        ResourceInfo res = new ResourceInfo();

        res.Path = strPath;
        //res.Prior = prior;
        res.fnCallBack = callback;

        RequestLoad(res);

        return res;
    }


    private IEnumerator CoLoadProcess()
    {
        while (m_bActive)
        {
            long elapsed = System.DateTime.Now.Ticks - m_nLoadBeginTime;
            if (elapsed > 5000000)
            {
                yield return null;
                m_nLoadBeginTime = System.DateTime.Now.Ticks;
            }

            while (m_ListRequest.Count > 0)
            {
                RequestInfo request = m_ListRequest[0];
                m_ListRequest.RemoveAt(0);
                if (request.res != null)
                {
                    if (request.eType == RequestInfo.eReqType.REQ_LOAD)
                    {
                        request.res.LoadAsync();
                    }
                    //else if(request.eType == RequestInfo.eReqType.REQ_FREE)
                    //{
                    //
                    //}
                }

                if (System.DateTime.Now.Ticks - m_nLoadBeginTime > 5000000)
                {
                    yield return null;
                    m_nLoadBeginTime = System.DateTime.Now.Ticks;
                }
            }

            yield return null;
        }
    }

    public void RequestProcessFinish(ResourceInfo res)
    {
        if (res.fnCallBack != null)
        {
            //无论是否加载成功都进行回调, res.Obj != null需要到回调函数中去判断，解决有些资源不存在，上层一直等的问题
            res.fnCallBack(res);
        }
    }

    // 发起协程载入请求
    private void RequestLoad(ResourceInfo res)
    {
        RequestInfo request = new RequestInfo();
        request.res = res; //后续改成对象池
        request.res.Request = request;
        request.eType = RequestInfo.eReqType.REQ_LOAD;
        request.fnRequestCallback = RequestProcessFinish;

        m_ListRequest.Add(request);
    }

    public void RequestFree(ResourceInfo res)
    {
        //释放资源还没有调用过 此处默认优先级最高
        res.Prior = eReqPrior.REQ_PRIOR_AT_ONCE;

        RequestInfo request = new RequestInfo();
        request.res = new ResourceInfo(); //后续改成对象池
        request.eType = RequestInfo.eReqType.REQ_FREE;

        m_ListRequest.Add(request);
    }
}
