/*
*	author: Maximilian Stecklina, Markus Rudolph 
*	Script for control the mobil menu
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuShow : MonoBehaviour
{
    //privat variables
	private bool show_menu;

	//public variables
	public Camera Usercam;


    // Use this for initialization
    void Start()
    {
        show_menu = GameObject.Find("menu_center").GetComponent<Canvas>().enabled;
    }

    // Update is called once per frame
    void Update()
    {
		// FROM PREFAB
        // Example: get controller's current orientation:
        Quaternion ori = GvrControllerInput.Orientation;

        // If you want a vector that points in the direction of the controller
        // you can just multiply this quat by Vector3.forward:
        Vector3 vector = ori * Vector3.forward;

        // ...or you can just change the rotation of some entity on your scene
        // (e.g. the player's arm) to match the controller's orientation
        //playerArmObject.transform.localRotation = ori;

        // Example: check if touchpad was just touched
        if (GvrControllerInput.TouchDown)
        {
            // Do something.
            // TouchDown is true for 1 frame after touchpad is touched.
        }

        // Example: check if app button was just released.
        if (GvrControllerInput.AppButtonDown)
        {
            // Do something.
            // AppButtonUp is true for 1 frame after touchpad is touched.
     
            //close ComponentMenu everytime
            GameObject.Find("compCanvas").GetComponent<Canvas>().enabled = false;

            GameObject.Find("ComponentController").GetComponent<ComponentController>().CloseCompCanvas();

            CanvasGroup cangroup = this.GetComponent<CanvasGroup>();
            //float alpha = cangroup.alpha;


            if (!show_menu)
            {
	            show_menu = true;

				//disable tunneling
				GameObject.Find("User").GetComponent<DaydreamElements.Tunneling.FirstPersonTunnelingLocomotion>().enabled = !show_menu;

            
                //menu in front of user
                Quaternion q = Usercam.transform.rotation;
                Vector3 orientation = q.eulerAngles;
                //orientation.z = 0;
                this.GetComponent<RectTransform>().rotation = Quaternion.Euler(orientation.x, orientation.y, 0);
                
                //Debug.Log("q = " + q);

                //find canvases
                GameObject menu_object = GameObject.Find("menu_center");
                GameObject map_object = GameObject.Find("map");
                GameObject settings_object = GameObject.Find("settings");

                //enable menu
                able_canvas(menu_object, show_menu);

                //enable BoxCollider
                able_BoxCollider(menu_object, show_menu);
               
                // set visibility
                cangroup.alpha = 0.85F;


            }

            else
            {
                show_menu = false;

				//enable tunneling
				GameObject.Find("User").GetComponent<DaydreamElements.Tunneling.FirstPersonTunnelingLocomotion>().enabled = !show_menu;

                //find all Canvas
                GameObject menu_object = GameObject.Find("menu_center");
                GameObject map_object = GameObject.Find("map");
                GameObject settings_object = GameObject.Find("settings");

                //close all canvas
                able_canvas(menu_object, show_menu);
                able_canvas(map_object, show_menu);
                able_canvas(settings_object, show_menu);

                // disable all boxcollider
                able_BoxCollider(menu_object, show_menu);
                able_BoxCollider(map_object, show_menu);
                able_BoxCollider(settings_object, show_menu);

                //set visibility
                cangroup.alpha = 0;


                // !!!!!!!!!!!!!!!!! ZURÜCKSCHIEBEN !!!!!!!!!!!!!!!!!!!!!
                //reset menu pose 
                GameObject center_can = GameObject.Find("menu_center");
                center_can.transform.localPosition = new Vector3(0, 0, 85);
                center_can.transform.localRotation = Quaternion.Euler(0, 0, 0);
			
            }

            //Debug.Log("alpha = " + alpha);
        }

        // Example: check the position of the user's finger on the touchpad
        if (GvrControllerInput.IsTouching)
        {
            Vector2 touchPos = GvrControllerInput.TouchPos;
            // Do something.
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
                Debug.Log("bla");
            }
            else
            {
               obj_can.enabled = enable;
               able_BoxCollider(obj, enable);
            }

        }
    }
}
