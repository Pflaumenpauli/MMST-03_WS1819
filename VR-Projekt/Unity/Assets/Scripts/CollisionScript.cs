/*
*	author: Maximilian Stecklina
*	Script for move component canves, if canvas collided with something
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
	private int i = 0;

	/*
	* Use this for initialization
	*/
	void Start ()
	{
		
	}
	
	/* 
	 * Update is called once per frame
	 */
	void Update ()
	{
	}

	/*
	 * use this when Collider collided with an rigidbody
	 */
	void OnTriggerStay (Collider other)
	{

		if (other.gameObject.tag == "Component") {
			Debug.Log ("!!! Collision mit einem tag=Test !!!");
			Debug.Log ("Komponente = " + other.gameObject.name);


			// FIXME: use a better algorithem to find new position for canvas
			newPositionCompCanvas ();    
		}
	}

	/*
	 * function: set new position oof canvas, relative to old position
	 */

	void newPositionCompCanvas ()
	{
		Debug.Log ("Move Canvas " + i);
		i++;
		GameObject compCanvas = GameObject.Find ("compCanvas");
		Vector3 positionCan = compCanvas.transform.localPosition;

		Debug.Log ("positionCan = " + positionCan);
		compCanvas.transform.localPosition = new Vector3 (positionCan.x - 0.2F, positionCan.y, positionCan.z);
        
	}
}
