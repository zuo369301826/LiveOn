using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public delegate void OnLevelLoaded();

public class LevelManager : Singleton<LevelManager>
{
    const string EMPTY_SCENE_NAME = "Empty";

    private OnLevelLoaded m_OnLevelLoaded = null;

    public OnLevelLoaded LevelLoaded { set { m_OnLevelLoaded = value; } }

    public void LoadScene(string strScenePath, OnLevelLoaded callback=null)
    {
        m_OnLevelLoaded = callback;

        GameRoot.Instance.StartCoroutine(CoLoadSceneAsync(strScenePath));
    }

    public void UnloadScene()
    {

    }

    public IEnumerator CoLoadSceneAsync(string strSceneName)
    {
        LoadingWnd loadingWnd = UIManager.instance.GetWnd(typeof(LoadingWnd)) as LoadingWnd;
        if (loadingWnd != null)
        {

            //载入Empty.unity，卸载旧场景 0-10%
            AsyncOperation unloadScene = SceneManager.LoadSceneAsync(EMPTY_SCENE_NAME);
            while (unloadScene != null && !unloadScene.isDone)
            {
                float fUnloadSceneProgress = unloadScene.progress;
                loadingWnd.SetProcessPercent(fUnloadSceneProgress * 0.1f);

                yield return null;
            }

            //Todo: 事先需要先载入相关AssetBundle

            // 10-100%
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(strSceneName);
            while (loadScene != null && !loadScene.isDone)
            {
                float fLoadSceneProgress = loadScene.progress;

                loadingWnd.SetProcessPercent(0.1f + fLoadSceneProgress * 0.9f);

                yield return null;
            }

            if (m_OnLevelLoaded != null)
            {
                m_OnLevelLoaded();
                m_OnLevelLoaded = null;
            }
        }
    }
}
