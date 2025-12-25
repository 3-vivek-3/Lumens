using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Singleton
    public static SceneController instance;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Debug.LogError("Scene manager already exists.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    //[SerializeField] private LoadingOverlay loadingOverlay;
    //[SerializeField] private GameObject bootCamera;

    private Dictionary<string, string> loadedSceneBySlot = new();

    private bool isBusy = false;

    private Coroutine ExecutePlan(SceneTransitionPlan plan)
    {
        if(isBusy)
        {
            Debug.LogWarning("Scene change already in progress.");
            return null;
        }

        isBusy = true;
        return StartCoroutine(ChangeSceneRoutine(plan));
    }

    private IEnumerator ChangeSceneRoutine(SceneTransitionPlan plan)
    {
        // Remove scenes
        foreach (string slotKey in plan.ScenesToUnload)
        {
            yield return UnloadSceneRoutine(slotKey);
        }

        // Clear unused assets
        if (plan.ClearUnusedAssets) yield return CleanUnusedAssetsRoutine();

        // Add new scenes
        foreach (var kvp in plan.ScenesToLoad)
        {
            if (loadedSceneBySlot.ContainsKey(kvp.Key)) yield return UnloadSceneRoutine(kvp.Key);

            yield return LoadAdditiveSceneRoutine(kvp.Key, kvp.Value, plan.ActiveSceneName == kvp.Value);
        }

        isBusy = false;
    }

    private IEnumerator LoadAdditiveSceneRoutine(string slotKey, string sceneName, bool setActive)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (loadOp == null) yield break;
        
        loadOp.allowSceneActivation = false;
        while(loadOp.progress < 0.9f)
        {
            yield return null;
        }

        loadOp.allowSceneActivation = true;
        while(!loadOp.isDone)
        {
            yield return null;
        }

        if(setActive)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if(newScene.IsValid() && newScene.isLoaded) SceneManager.SetActiveScene(newScene);
        }

        loadedSceneBySlot[slotKey] = sceneName;
    }

    private IEnumerator UnloadSceneRoutine(string slotKey)
    {
        if (!loadedSceneBySlot.TryGetValue(slotKey, out string sceneName)) yield break;
        if(string.IsNullOrEmpty(sceneName)) yield break;

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        
        if(unloadOp != null)
        {
            while(!unloadOp.isDone)
            {
                yield return null;
            }
        }
        loadedSceneBySlot.Remove(slotKey);
    }

    private IEnumerator CleanUnusedAssetsRoutine()
    {
        AsyncOperation cleanOp = Resources.UnloadUnusedAssets();
        while(!cleanOp.isDone)
        {
            yield return null;
        }
    }

    public SceneTransitionPlan NewTransition()
    {
        return new SceneTransitionPlan();
    }

    public class SceneTransitionPlan
    {
        public Dictionary<string, string> ScenesToLoad { get; } = new();

        public List<string> ScenesToUnload { get; } = new();

        public string ActiveSceneName { get; private set; } = "";

        public bool ClearUnusedAssets { get; private set; } = false;

        public SceneTransitionPlan Load(string slotKey, string sceneName, bool setActive = false)
        {
            ScenesToLoad[slotKey] = sceneName;
            if (setActive) ActiveSceneName = sceneName;
            return this;
        }

        public SceneTransitionPlan Unload(string slotKey)
        {
            ScenesToUnload.Add(slotKey);
            return this;
        }

        public SceneTransitionPlan WithClearUnusedAssets()
        {
            ClearUnusedAssets = true;
            return this;
        }

        public Coroutine Perform()
        {
            return SceneController.instance.ExecutePlan(this);
        }
    }
}
