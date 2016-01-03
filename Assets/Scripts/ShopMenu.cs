using UnityEngine;
using System.Collections;
using UnityEditor;
using ChickenHunt.Scripts.FirstPerson;

public class ShopMenu : MonoBehaviour
{


    public GameObject shopMenu;

    private Placement placement;
    private FirstPersonController_ksi fps_ksi;

    void Start()
    {
        placement = GameObject.Find("PlacementObject").GetComponent<Placement>();
        fps_ksi = GameObject.Find("Player").GetComponent<FirstPersonController_ksi>();
        CloseMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
       
        if (!shopMenu.activeSelf)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        shopMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetMouseLookActive(true);
    }

    public void OpenMenu()
    {
        if (!placement.placingObject)
        {
            shopMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetMouseLookActive(false);
        }
    }

    private void SetMouseLookActive(bool state)
    {
        fps_ksi.MouseLookEnabled = state;
    }
}
