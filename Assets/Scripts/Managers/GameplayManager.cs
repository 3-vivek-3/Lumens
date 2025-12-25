using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    #region Singleton
    public static GameplayManager instance;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Debug.LogError("Gameplay manager already exists.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject spotlightPrefab;
    [SerializeField] private MazeManager mazeManager;
    [SerializeField] private GameObject ballCamera;
    [SerializeField] private Menu gameplayPauseMenu;

    public Transform dynamicContainer;

    private GameObject ball;
    private bool ballSpawned = false;
    private GameObject spotlight;

    private void Start()
    {
        ballCamera.transform.rotation = Quaternion.Euler(67.5f, 0f, 0f);
        mazeManager.GenerateMaze();
        SpawnBall();

        InputManager.instance.PauseEvent += PauseGame;
        InputManager.instance.ResumeEvent += ResumeGame;
        InputManager.instance.BackEvent += MenuBack;
    }

    private void LateUpdate()
    {
        if (ballSpawned && spotlight) spotlight.transform.position = ball.transform.position + new Vector3(0f, 8f, 0f);

        if (ballSpawned && ballCamera) ballCamera.transform.position = ball.transform.position + new Vector3(0f, 10f, -5f);
    }

    public void SpawnBall()
    {
        float coord = mazeManager.cellSize / 2f * (1 - mazeManager.mazeSize);

        ball = Instantiate(ballPrefab, new Vector3(coord, 5f, coord), Quaternion.identity, dynamicContainer);
        spotlight = Instantiate(spotlightPrefab, ball.transform.position, Quaternion.Euler(90f, 0f, 0f), dynamicContainer);
        ballSpawned = true;
        //Debug.Log("Ball spawned.");
    }

    public void PauseGame()
    {
        if (Time.timeScale == 0f) return;
        Time.timeScale = 0f;
        MenuManager.instance.OpenMenu(gameplayPauseMenu);
        InputManager.instance.SwitchToUIInput();
    }

    public void ResumeGame()
    {
        if (Time.timeScale == 1f) return;
        MenuManager.instance.CloseAllMenus();
        InputManager.instance.SwitchToGameplayInput();
        Time.timeScale = 1f;
    }

    public void MenuBack()
    {
        MenuManager.instance.CloseMenu();
        if (MenuManager.instance.IsMenuStackEmpty()) ResumeGame(); 
    }

    public void GameOver()
    {
        GlobalManager.instance.SwitchToMainMenu();
        Debug.Log("Game over.");
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
        InputManager.instance.PauseEvent -= PauseGame;
        InputManager.instance.PauseEvent -= ResumeGame;
        InputManager.instance.BackEvent -= MenuBack;
    }
}
