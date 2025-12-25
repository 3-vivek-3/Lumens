using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayPauseMenu : Menu
{
    [SerializeField] private Menu optionsMenu;

    public void OnResumePressed()
    {
         if(GameplayManager.instance) GameplayManager.instance.ResumeGame();
    }

    public void OnOptionsPressed()
    {
        MenuManager.instance.OpenMenu(optionsMenu);
    }

    public void OnExitPressed()
    {
        GlobalManager.instance.SwitchToMainMenu();
    }
}
