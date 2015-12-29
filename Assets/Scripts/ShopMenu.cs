using UnityEngine;
using System.Collections;
using UnityEditor;

public class ShopMenu : MonoBehaviour
{


    public GameObject shopMenu;

    private Placement placement;

    void Start()
    {
        placement = GameObject.Find("PlacementObject").GetComponent<Placement>();
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
    }

    public void OpenMenu()
    {
        if (!placement.placingObject)
        {
            shopMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
