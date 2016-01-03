using UnityEngine;
using System.Collections;

public class Arrow_projectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void OnCollisionEnter(Collision col){
			print("Collision");

		this.transform.parent = col.transform;
		this.transform.localPosition = col.transform.InverseTransformPoint(col.contacts[0].point);

		Destroy(this.GetComponent<Rigidbody>());
		Destroy(this.GetComponent<MeshCollider>());
			
	}
}
