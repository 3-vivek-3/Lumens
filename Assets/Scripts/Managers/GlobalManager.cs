using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneController;

public class GlobalManager : MonoBehaviour
{
    #region Singleton
    public static GlobalManager instance;
    private void Awake()
    {
        if (instance)
        {
            Debug.LogError("Global manager already exists.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private LoadingOverlay loadingOverlay;
    [SerializeField] private GameObject bootCamera;
    /// <summary>
    /// For boot camera, I set the viewport rect's width and height to 0.
    /// </summary>

    public void Start()
    {
        // Load menu scene
        SwitchToMainMenu();
    }

    public void SwitchToGameplay()
    {
        var plan = SceneController.instance.NewTransition()
            .Load(SceneDatabase.Slots.Gameplay, SceneDatabase.Scenes.Gameplay, true)
            .Unload(SceneDatabase.Slots.Menu);

        StartCoroutine(Switch(plan, () => {InputManager.instance.SwitchToGameplayInput();}));
    }

    public void SwitchToMainMenu()
    {
        var plan = SceneController.instance.NewTransition()
            .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.Menu, true)
            .Unload(SceneDatabase.Slots.Gameplay);

        StartCoroutine(Switch(plan, () => {InputManager.instance.SwitchToUIInput();}));
    }

    private IEnumerator Switch(SceneTransitionPlan plan, Action onTransitionComplete)
    {
        InputManager.instance.DisableAllInput();
        Time.timeScale = 1f;

        AudioManager.instance.FadeOutMusic();

        //bootCamera.SetActive(true);
        yield return loadingOverlay.FadeInBlack();

        MenuManager.instance.CloseAllMenus();

        yield return plan.Perform();

        AudioManager.instance.FadeInMusic();

        yield return loadingOverlay.FadeOutBlack();
        //bootCamera.SetActive(false);

        onTransitionComplete?.Invoke();
    }
}
