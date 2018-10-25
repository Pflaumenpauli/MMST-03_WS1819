/*
*	author: Maximilian Stecklina, Markus Rudolph 
*	Script for returning the informationen of the material, if the pointer create events
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentColor : MonoBehaviour {

    public Material selectedMaterial;
    public Material normalMaterial;
    public Material hoveringMaterial;
    
	/*
	*	Use this for initialization
	*/
    void Start () {
		
	}
	
	/* 
	* 	Update is called once per frame
	*/
	void Update () {
		
	}


    // GET Materials

	/*
	* 	get the selectedMaterial of the pointer
	*	@return give the selectedMaterial
	*/
    public Material getSelectedMaterial()
    {
        return selectedMaterial;
    }

	/*
	* 	get the NormalMaterial of the pointer
	*	@return give the normalMaterial
	*/
    public Material getNormalMaterial()
    {
        return normalMaterial;
    }
	
	/*
	* 	get the HoveringMaterial of the pointer
	*	@return give the hoveringMaterial
	**/
    public Material getHoveringMaterial()
    {
        return hoveringMaterial;
    }
}
