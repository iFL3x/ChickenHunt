using UnityEngine;
using System.Collections;
using ChickenHunt.Scripts.FirstPerson;

public class Ice_puddle : MonoBehaviour {

	public int speedReducer;


	void OnTriggerEnter(Collider col){
		//Only A Player can get slowed
		if(col.transform.tag == "Player")
		{
			Debug.Log("Player: " + col.name + " is slowed");
			GameObject playerObject = col.gameObject;
			playerObject.GetComponent<FirstPersonController_ksi>().reduceWalkSpeed(speedReducer);

		}
	}

	void OnTriggerExit(Collider col){
		//Only A Player can get slowed
		if(col.transform.tag == "Player")
		{
			GameObject playerObject = col.gameObject;
			//Set the speed to normal
			playerObject.GetComponent<FirstPersonController_ksi>().increaseWalkSpeed(speedReducer);

		}
	}
}
