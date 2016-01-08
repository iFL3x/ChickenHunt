using UnityEngine;
using System.Collections;


/**
 * This script is attached to an arrow projectile.
 * Every arrow needs a Rigidbody with default:
 * gravity = false
 * kinematic = false
 * mass = 0,1
 * 
 * And a mesh collider witch is not active at first.
 * 
 * 
 **/
public class Arrow_projectile : MonoBehaviour {

	public int destory_delay = 3;
	public GameObject spawnEffect;

	private GameObject effect;

	//Remove object after a certant delay.
	void destoryAfterHit(GameObject go){
		Destroy(go,destory_delay);
	}


	void OnCollisionEnter(Collision col){
		Debug.Log("Arrow hit: " +col.gameObject.name +".");
		//Pin the object to the one it's hitting.
		this.transform.parent = col.transform;
		this.transform.localPosition = col.transform.InverseTransformPoint(col.contacts[0].point);
		//Remove Physic objects and start the destroy countdown
		Destroy(this.GetComponent<Rigidbody>());
		Destroy(this.GetComponent<MeshCollider>());
		destoryAfterHit(this.gameObject);

		//Spawns the effect only on the ground
		if(col.transform.tag == "Ground"){
			//Spawns a effect from an arrow
			if(spawnEffect != null){
				effect = (GameObject) Instantiate(spawnEffect,this.transform.position,new Quaternion(0,180,180,0));
				destoryAfterHit(effect);
			}
		}
	}
}
