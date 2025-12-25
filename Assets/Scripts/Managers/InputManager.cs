using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameControls.IGameplayActions, GameControls.IUIActions
{
    public static InputManager instance;
    private GameControls gameControls;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Debug.LogError("Input manager already exists.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        gameControls = new GameControls();

        gameControls.Gameplay.SetCallbacks(this);
        gameControls.UI.SetCallbacks(this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public event Action<Vector2> MoveEvent;
    public event Action PauseEvent;
    public event Action ResumeEvent;
    public event Action BackEvent;

    public void DisableAllInput()
    {
        gameControls.Gameplay.Disable();
        gameControls.UI.Disable();
    }

    public void SwitchToGameplayInput()
    {
        gameControls.Gameplay.Enable();
        gameControls.UI.Disable();
    }

    public void SwitchToUIInput()
    {
        gameControls.Gameplay.Disable();
        gameControls.UI.Enable();
    }
    
    void GameControls.IGameplayActions.OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void GameControls.IGameplayActions.OnPause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            PauseEvent?.Invoke();
        }
    }

    void GameControls.IUIActions.OnResume(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            ResumeEvent?.Invoke();
        }
    }

    void GameControls.IUIActions.OnBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BackEvent?.Invoke();
        }
    }
}
