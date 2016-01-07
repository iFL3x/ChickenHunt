using UnityEngine;
using System.Collections;
using ChickenHunt.Scripts.FirstPerson;

public class Arrow_reload_object : MonoBehaviour {


    public int ammo_count;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    //Fills the ammo count of the Player how picks the item up.
    void OnCollisionEnter(Collision col)
    {

        if(col.transform.tag == "Player")
        {
            GameObject playerObject = col.gameObject;
            playerObject.GetComponent<FirstPersonController_ksi>().refillArrows(ammo_count);
            Destroy(this.gameObject);

        }
    }
}
