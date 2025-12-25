using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject ROOT;
    [SerializeField] private GameObject firstSelectedObject;
    [SerializeField] private bool openOnStart = false;

    virtual protected void Start()
    {
        if (openOnStart) MenuManager.instance.OpenMenu(this);
    }

    virtual public void TurnOn()
    {
        if (!ROOT) Debug.LogError("ROOT not assigned in " + this.name);
        ROOT.SetActive(true);

        if(firstSelectedObject) EventSystem.current.SetSelectedGameObject(firstSelectedObject);
    }

    virtual public void TurnOff()
    {
        if(!ROOT) Debug.LogError("ROOT not assigned in " + this.name);
        ROOT.SetActive(false);
    }

    public GameObject GetFirstSelectedObject()
    {
        return firstSelectedObject;
    }
}
