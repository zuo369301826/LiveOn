using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UIType
{
    UI3D,
    UI2D
};

public abstract class UIBaseWnd
{
    public class ExceptInfo
    {
        public System.Type m_Type;
        public string m_strOpenAnim = null;
        public string m_strCloseAnim = null;
    }

    public enum State
    {
        CLOSED,
        OPENING,
        READY
    }

    protected UIType type;

    protected string m_strPrefabPath;

    public string PrefabPath
    {
        get { return m_strPrefabPath; }
    }
    
    public State state { get; set; }

    public bool IsVisible { get; protected set; }

    public GameObject root { get; protected set; }

    public GameObject parentObject { get; protected set; }

    public UIBaseWnd()
    {

    }

    public UIBaseWnd Register(string strPrefabPath, UIType type, GameObject parentObj = null, bool bIsNeedWaitingPanel = false, eReqPrior prior = eReqPrior.REQ_PRIOR_HIGH, bool bIsResources = false)
    {
        m_strPrefabPath = strPrefabPath;
        this.type = type;
        this.parentObject = parentObj;
        return this;
    }

    public virtual void Init()
    {

    }

    public virtual void Term()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void LateUpdate()
    {

    }

    protected virtual void OnLoaded()
    {

    }

    protected virtual void OnDisplay()
    {

    }

    protected abstract void OnOpen();

    protected virtual void OnClose()
    {
        if (root.activeSelf)
            root.SetActive(false);
    }

    protected virtual bool OnHide()
    {
        return false;
    }

    public virtual void Show(bool visible)
    {
        IsVisible = visible;

        switch (state)
        {
            case State.CLOSED:
                if (IsVisible)
                {
                    state = State.OPENING;
                    //DoPreload();
                    ResourceManager.instance.LoadResource(m_strPrefabPath, OnPrefabLoaded);
                }
                break;
            case State.READY:
                if (IsVisible)
                {
                    root.SetActive(true);
                    OnDisplay();
                    //NoviceGuideManager.Instance.TryCheckNoviceGuide(CheckNoviceGuideType.ShowWnd, GetType().ToString());
                }
                else
                {
                    if (!OnHide())
                    {
                        OnClose();
                    }
                }
                break;
        }
    }
    public virtual void Close()
    {
        if (root.activeSelf)
            root.SetActive(false);
    }

    // 事件绑定
    protected abstract void AddEventListener();

    // 每次界面打开时候，刷新界面
    public abstract void RefreshUI();

    protected virtual void OnPrefabLoaded(ResourceInfo resourceInfo)
    {
        if (root != null)
        {
            GameObject.Destroy(root);
            root = null;
        }

#if UNITY_EDITOR
        if (resourceInfo.Obj == null)
        {
            Debug.LogError("Failed to load " + resourceInfo.Path + ".");
            return;
        }
#endif
        GameObject tmpGO = resourceInfo.Obj as GameObject;
        root = Object.Instantiate(tmpGO) as GameObject;
        root.name = tmpGO.name;

        if (type == UIType.UI2D)
        {
            UIManager.instance.Attach(root);
        }         
        else if (type == UIType.UI3D)
        {
            UIManager.instance.AttachToObject(root, parentObject);
        }

        //if (root != null)
        //    m_Animator = root.GetComponent<Animator>();

        OnOpen();

        state = State.READY;

        AddEventListener();
        RefreshUI();

        ////如果是预加载的界面 且界面状态为不显示 
        //if (!isVisible && m_bPreloadWnd)
        //    root.gameObject.SetActive(false);
        //else
        //    Show(isVisible);

        //m_bPreloadWnd = false;
    }

    public void SetParentObject(GameObject obj)
    {
        parentObject = obj;
    }

}