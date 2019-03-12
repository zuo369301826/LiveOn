using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<System.Type, UIBaseWnd> m_Windows = new Dictionary<System.Type, UIBaseWnd>();

    private Transform m_UIRoot = null;

    private Camera m_UICamera2D = null;

    public void Init()
    {
        ResourceInfo srcInfo = ResourceManager.instance.LoadResource("Assets/UI/NGUIRoot/NGUIRoot.prefab");

        if (srcInfo == null)
            return;
        GameObject uiroot = GameObject.Instantiate(srcInfo.Obj) as GameObject;
        if (uiroot != null)
        {
            GameObject.DontDestroyOnLoad(uiroot);
            uiroot.name = srcInfo.Obj.name;
            m_UIRoot = uiroot.transform;

            m_UICamera2D = uiroot.GetComponentInChildren<Camera>();
        }

        Window<LoginWnd>().Register("Assets/UI/UIPrefab/Login/LoginWnd.prefab", UIType.UI2D);
        Window<LoadingWnd>().Register("Assets/UI/UIPrefab/Loading/LoadingWnd.prefab", UIType.UI2D);
    }

    private UIBaseWnd Window<T>() where T : UIBaseWnd, new()
    {
        UIBaseWnd window = new T();
        m_Windows.Add(typeof(T), window);
        return window;
    }

    public void Term()
    {

    }

    public void Tick()
    {
        Dictionary<System.Type, UIBaseWnd>.Enumerator it = m_Windows.GetEnumerator();
        while (it.MoveNext())
        {
            UIBaseWnd wnd = it.Current.Value;
            if(wnd.IsVisible)
            {
                it.Current.Value.Update();
            }
        }
    }

    public void LateTick()
    {
        Dictionary<System.Type, UIBaseWnd>.Enumerator it = m_Windows.GetEnumerator();
        while (it.MoveNext())
        {
            UIBaseWnd wnd = it.Current.Value;
            if (wnd.IsVisible)
            {
                it.Current.Value.LateUpdate();
            }
        }
    }

    public void Attach(GameObject wndRoot)
    {
        wndRoot.transform.SetParent(m_UIRoot.GetComponentInChildren<Camera>().transform, false);
        //Canvas wndCanvas = wndRoot.GetComponent<Canvas>();
        //if (wndCanvas != null)
        //{
        //    wndCanvas.worldCamera = m_UICamera2D;
        //}
    }
    
    public void AttachToObject(GameObject wndRoot, GameObject obj)
    {
        wndRoot.transform.SetParent(obj.transform, false);
    }

    public UIBaseWnd GetWnd(System.Type type)
    {
        UIBaseWnd ret = null;
        m_Windows.TryGetValue(type, out ret);
        return ret;
    }

    public void CloseAllWnd()
    {
        Dictionary<System.Type, UIBaseWnd>.Enumerator it = m_Windows.GetEnumerator();
        while (it.MoveNext())
        {
            UIBaseWnd wnd = it.Current.Value;
            if (wnd.IsVisible && wnd.state == UIBaseWnd.State.READY)
            {
                wnd.Show(false);
            }
        }
    }

    public void DestroyAllWnd()
    {
        Dictionary<System.Type, UIBaseWnd>.Enumerator it = m_Windows.GetEnumerator();
        while (it.MoveNext())
        {
            UIBaseWnd wnd = it.Current.Value;
            wnd.Term();
            wnd.Close();
        }
        m_Windows.Clear();
    }
}