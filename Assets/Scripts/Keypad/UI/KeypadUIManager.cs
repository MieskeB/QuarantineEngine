using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadUIManager : MonoBehaviour
{
    public static KeypadUIManager Instance { get; private set; }

    [SerializeField] private GameObject keypadUIPrefab;
    private GameObject currentUI;
    private KeypadController currentController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenKeypad(KeypadController controller)
    {
        currentController = controller;

        if (currentUI == null)
        {
            currentUI = Instantiate(keypadUIPrefab, transform);
            currentUI.GetComponent<KeypadUI>().Init(controller);
        }
    }

    public void CloseKeypad()
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }

        MouseLook look = FindObjectOfType<MouseLook>();
        if (look != null)
        {
            look.ExitFocusMode();
        }
    }
}
