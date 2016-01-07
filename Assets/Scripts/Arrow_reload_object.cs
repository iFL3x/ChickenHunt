using UnityEngine;
using System.Collections;
using ChickenHunt.Scripts.FirstPerson;

public class Arrow_reload_object : MonoBehaviour {


    public int ammo_count;

	//Give ammo to the player and destory itself.
	void OnTriggerEnter(Collider col){
		//Only A Player can get Ammo
		if(col.transform.tag == "Player")
		{
			GameObject playerObject = col.gameObject;
			playerObject.GetComponent<FirstPersonController_ksi>().refillArrows(ammo_count);
			Destroy(this.gameObject);

		}
	}
}
