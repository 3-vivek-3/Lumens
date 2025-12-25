using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    #region Singleton
    public static MenuManager instance;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Debug.LogError("Menu manager already exists.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // If the stack is empty, then no menu is open.
    private Stack<(Menu, GameObject)> menuStack = new();

    public void OpenMenu(Menu menu)
    {
        GameObject previousSelectedObject;

        // Don't accidentally open the menu twice.
        if (menuStack.Count > 0 && menuStack.Peek().Item1 == menu) return;

        if(menuStack.Count > 0)
        {
            previousSelectedObject = EventSystem.current.currentSelectedGameObject;
            menuStack.Peek().Item1.TurnOff();
        }
        else previousSelectedObject = null;

        menu.TurnOn();
        menuStack.Push((menu, previousSelectedObject));
    }

    public void CloseMenu()
    {
        if (menuStack.Count <= 1) return;

        var (topMenu, previousSelectedObject) = menuStack.Pop();

        topMenu.TurnOff();

        menuStack.Peek().Item1.TurnOn();

        // If the previous selected object is null
        if (previousSelectedObject) EventSystem.current.SetSelectedGameObject(previousSelectedObject);
        else EventSystem.current.SetSelectedGameObject(menuStack.Peek().Item1.GetFirstSelectedObject());
    }

    public void CloseAllMenus()
    {
        foreach(var (menu, _) in menuStack)
        {
            menu.TurnOff();
        }

        menuStack.Clear();
    }

    public bool IsMenuStackEmpty()
    {
        return menuStack.Count == 0;
    }
}
