using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    [SerializeField] private Menu optionsMenu;
    public void OnPlayPressed()
    {
        GlobalManager.instance.SwitchToGameplay();
    }

    public void OnOptionsPressed()
    {
        MenuManager.instance.OpenMenu(optionsMenu);
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
