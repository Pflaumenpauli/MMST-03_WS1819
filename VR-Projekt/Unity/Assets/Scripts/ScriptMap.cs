/*
*	author: Maximilian Stecklina, Markus Rudolph
*	Script to control all function of the map in the mobil menu
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptMap : MonoBehaviour
{
	//public var
    public Camera MainCamera;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		//update user pose on map
        setUserPose();
    }

	/*
	 * getUserPose: helper function 
	 * @return: the position of the MainCamera
	 */
    private Vector3 getUserPose()
    {
        Vector3 v = MainCamera.transform.position;
        return v;
    }


	/*
	 * setUserPose: update the current user position and orientation to the map 
	 */
    public void setUserPose()
    {

        // Get size of rooms-collider
        Collider m_Collider;
        Vector3 m_Size;

        //Fetch the Collider from the GameObject
        m_Collider = GameObject.Find("Raum").GetComponent<Collider>();

        //Fetch the size of the Collider volume
        m_Size = m_Collider.bounds.size;

        // get position user
        Vector3 v_user = getUserPose();
        Vector3 v_walkplane = GameObject.Find("Walkplane").transform.position;
        Vector3 v_new = v_user - v_walkplane;


        // size of map-canvas
        float map_image_width = 900; // TODO dynamischer Abruf der Höhe des Bildes
        float map_image_height = 550;  // derzeit hart reinprogrammiert

        // MAX - Formel (einfaches Mapping durch ausprobieren der Werte)
        float helpx = map_image_width / (40.3F + 91) * v_new.x + 91 * map_image_width / (40.3F + 91) - map_image_width/2;
        float helpy = map_image_height / (27.5F + 49) * v_new.z + 49 * map_image_height / (27.5F + 49) - map_image_height/2;


        // set new position fot the dragon
        GameObject.Find("dot_user").transform.localPosition = new Vector3(helpx, helpy, 0);
        GameObject.Find("dot_orientation").transform.localPosition = new Vector3(helpx, helpy, 0);
        GameObject.Find("dot_orientation").transform.localEulerAngles = new Vector3(0, 0, -MainCamera.transform.rotation.eulerAngles.y);
    	  	//Debug.Log("Orientierung: " + MainCamera.transform.rotation.eulerAngles);

        	//Debug.Log("dot_user localposition:" + GameObject.Find("dot_user").transform.localPosition);
        }

    /**
     * setMarkerOnMap: function to set a marker on map
     * DONT WORK! only a try
     * don't use in the moment
     */
    public void setMarkerOnMap()
    {
        //Debug.Log("Hit me Baby one more time");

        // get Collision-Point with Canvas
        //GameObject.Find("map").GetComponent<BoxCollider>().
        Vector3 pointerPosLocal = new Vector3(0,0,0);

        // highlight this spot on the map (optional)

        // map to real world / plant
        float map_image_width = 900; // TODO dynamischer Abruf der Höhe des Bildes
        float map_image_height = 550;  // derzeit hart reinprogrammiert
        float roomX = (40.3F + 91) / map_image_width * pointerPosLocal.x + ( (40.3F + 91)/2 - 91 );
        float roomY = (27.5F + 49) / map_image_height * pointerPosLocal.y - ( (27.5F + 49)/2 - 49 );

        // set position to marker-spot 
       // GameObject.Find("MapMarker").transform.localPosition = new Vector3(-5000, 80, 6500); // test
    }
}