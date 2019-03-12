using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public void Init()
    {
        ResourceManager.instance.Init();

        AssetBundleManager.instance.Init();

        AssetsManager.instance.Init();

        UIManager.instance.Init();   

        LoadFirstResources();

        StartPreloadProcess();
    }

    public void Term()
    {

    }

    public void Tick()
    {
        ResourceManager.instance.Tick();

        UIManager.instance.Tick();
    }

    public void LateTick()
    {
        UIManager.instance.LateTick();
    }

    private void LoadFirstResources()
    {
        ShowLoadingWnd();
    }

    private void ShowLoadingWnd()
    {
        UIManager.instance.GetWnd(typeof(LoadingWnd)).Show(true);
    }

    private void SceneLoadingFinished()
    {
        UIManager.instance.GetWnd(typeof(LoadingWnd)).Show(false);
    }

    private void StartPreloadProcess()
    {
        GameRoot.Instance.StartCoroutine(CoPreloadProcess());
    }

    private IEnumerator CoPreloadProcess()
    {
        yield return null;

        LoadingWnd loadingWnd = UIManager.instance.GetWnd(typeof(LoadingWnd)) as LoadingWnd;
        if (loadingWnd != null)
        {
            while (!loadingWnd.IsVisible)
            {
                Debug.Log("loading window not loaded");
                yield return null;
            }

            //载入配置文件
            Debug.Log("Step 1 载入配置文件");

            yield return null;

            //预加载资源

            Debug.Log("Step 2 预加载资源");

            yield return null;

            //载入登录界面
            Debug.Log("Step 3 载入登录界面");

            UIManager.instance.GetWnd(typeof(LoginWnd)).Show(true);

            yield return null;

            yield return LevelManager.instance.CoLoadSceneAsync("Garage");

            Debug.Log("Step All 载入完成");

            yield return null;

            //测试代码
            while (loadingWnd.GetProcessPercent() < 1.0f)
            {
                loadingWnd.SetProcessPercent(loadingWnd.GetProcessPercent() + 0.01f);
                yield return null;
            }
            yield return WaitForLoginWndLoaded();

            loadingWnd.Show(false);
        }
    }

    private IEnumerator WaitForLoginWndLoaded()
    {
        LoginWnd loginWnd = UIManager.instance.GetWnd(typeof(LoginWnd)) as LoginWnd;
        while (!loginWnd.IsVisible)
        {
            Debug.Log("loginWnd not loaded");
            yield return null;
        }
    }

    public void LoadMatchScene(string strSceneName)
    {
        UIManager.instance.GetWnd(typeof(LoadingWnd)).Show(true);

        LevelManager.instance.LoadScene(strSceneName, SceneLoadingFinished);
    }
}
