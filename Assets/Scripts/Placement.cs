using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour
{

    private GameObject PlacementObject;
    //public GameObject PlacementObjectValidator;
    public float PlacementObjectDistance = 4f;

    public bool placingObject = false;


	// Use this for initialization
	void Start () {
	    //-- Buying/Getting Object
	    /*placingObject = true;
        PlacementObject.transform.SetParent(transform);
        PlacementObject.transform.localPosition = new Vector3(0,0,PlacementObjectDistance);
	    PlacementObject.tag = "PlacementObject";*/
	    /* GameObject validator = (GameObject) Instantiate(PlacementObjectValidator, PlacementObject.transform.position, PlacementObject.transform.rotation);
	    validator.name = "PlacementObjectValidator";
        validator.transform.SetParent(PlacementObject.transform);*/
	    //--
	}

    void Update()
    {
        
    }

    public void PlaceNewItem(GameObject itemPrefab)
    {
        placingObject = true;
        GameObject item = (GameObject) Instantiate(itemPrefab);
        PlacementObject = item;
        PlacementObject.transform.SetParent(transform);
        PlacementObject.transform.localPosition = new Vector3(0, 0, PlacementObjectDistance);
        PlacementObject.tag = "PlacementObject";
    }
	

}
