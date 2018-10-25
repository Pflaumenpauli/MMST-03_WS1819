/*
*	author: Maximilian Stecklina, Markus Rudolph
*	Script to slide the mobile menu if a submenu is choosen
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

	/*
	 * slide_left: slide canvas of mobil menu to the left and oben new one infront
	 * UPDATE now slide menu up, not left
	 */
    public void slide_left(GameObject canobj)
    {
        Canvas can = canobj.GetComponent<Canvas>();

        	//Debug.Log("sleiden :)");
        Vector3 v = this.transform.localPosition;
        	//Debug.Log("sleiden :)" + v);

        // safe state of can.enable
        bool can_open = can.enabled;

        // Close all Canvas (map, settings, ...) BUT not center_menu
        GameObject map_object = GameObject.Find("map");
        GameObject settings_object = GameObject.Find("settings");


        //close all canvas
        able_canvas(map_object, false);
        able_canvas(settings_object, false);

        //disable all boxcollider
        able_BoxCollider(map_object, false);
        able_BoxCollider(settings_object, false);

        if (can_open == false)
        {
            //set pose

            //this.transform.localPosition = new Vector3(-86.3F, 0, 62);    //slide left
            //this.transform.localRotation = Quaternion.Euler(0, -30, 0);
            
            //
            this.transform.localPosition = new Vector3(0, 37.7F, 80.7F);  //slide up
            this.transform.localRotation = Quaternion.Euler(-25, 0, 0);

            	//Debug.Log("new position:" + this.transform.localPosition);

            //open map
            can.enabled = true;
            able_BoxCollider(canobj, true);
        }
        else
        {
            //set pose back
            this.transform.localPosition = new Vector3(0, 0, 85);
            this.transform.localRotation = Quaternion.Euler(0, 0, 0);
            	//Debug.Log("new position:" + this.transform.localPosition);

            //close map
            can.enabled = false;

            
        }

		//FIXME : new function if new canvas is the map canvas
        if (can == GameObject.Find("map").GetComponent<Canvas>()) 
        {

        }

    }

	/*
	* able_canvas: enable/disable canvas and all boxcollider
	*/
    private void able_canvas(GameObject obj, bool enable)
    {
        if (obj != null)
        {
            //If we found the object , get the Canvas component from it.
            Canvas obj_can = obj.GetComponent<Canvas>();
            if (obj_can == null)
            {
                //Debug.Log("bla");
            }
            else
            {
                obj_can.enabled = enable;
                able_BoxCollider(obj, enable);
            }

        }
    }

	/*
	* able_BoxCollider: help function to disable boxcollider from obj and all children
	*/
    private void able_BoxCollider(GameObject obj, bool enable)
    {
        BoxCollider[] boxes = obj.GetComponentsInChildren<BoxCollider>();

        for (var i = 0; i < boxes.Length; i++)
        {
            boxes[i].enabled = enable;
        }
    }
}
