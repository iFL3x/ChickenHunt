using UnityEngine;
using System.Collections;

public class PlacementObject : MonoBehaviour
{
    public bool objectPlaced;
    public bool placeValid;
    public bool objectStackable;
    public bool allowHeightPlacement;

    private Color colorValid = Color.green;
    private Color colorNotValid = Color.red;
    

    private float heightMoveStep = 0.2f;
    private float maxHeight = 50f;
    private float minHeight = 0f;

    private Placement placement;

    void Start()
    {
        colorValid = GetComponent<Renderer>().material.color;
        if (!objectPlaced)
        {
            placement = GameObject.Find("PlacementObject").GetComponent<Placement>();
        }

    }

    void Update()
    {

        if (objectPlaced)
        {
            Destroy(GetComponent<PlacementObject>());
        }

        if (Input.GetMouseButtonDown(1))
        {
            transform.Rotate(new Vector3(0,90,0));
        }

        if (allowHeightPlacement)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && transform.position.y <= maxHeight)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + heightMoveStep, transform.position.z);
            } 
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.y >= minHeight)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - heightMoveStep, transform.position.z);
            }
        }

        if (Input.GetMouseButtonDown(0) && placeValid)
        {
            transform.parent = null;
            objectPlaced = true;
            placement.placingObject = false;
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (objectPlaced)
        {
            return;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Placement"))
        {
            SetPlaceValid(false);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Placement"))
        {
            SetPlaceValid(true);
        }
    }

    private void SetPlaceValid(bool state)
    {
        placeValid = state;
        if (placeValid)
        {
            GetComponent<Renderer>().material.color = colorValid;
        }
        else
        {
            GetComponent<Renderer>().material.color = colorNotValid;
        }
    }
}











/* in trigger enter
              AUTO SNAPPING TRY (Height placement on collision) Not working as excpected yet

              if (col.transform.CompareTag("PlacementObject"))
              {
                  Debug.Log(col.transform.name);
                  placeValid = false;
                  lastHitObject = col.transform;
                  transform.position = new Vector3(transform.position.x, transform.position.y + lastHitObject.GetComponent<Collider>().bounds.size.y + 0.04f, transform.position.z);
                  placeValid = true;
        }*/
