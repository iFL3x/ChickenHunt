using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ChickenHunt.Scripts.FirstPerson;

public class UIManager : MonoBehaviour {

    public Text amunition_panel;
	public Text health_panel;

    private FirstPersonController_ksi player;

	// Use this for initialization
	void Start () {

        player = GetComponent<FirstPersonController_ksi>();
	}
	
	// Update is called once per frame
	void Update () {
        amunition_panel.text = player.amunition.ToString();
		health_panel.text = player.health.ToString();
	}

}
