/*
*	author: Maximilian Stecklina  
*	Script to manage the P & I charts
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowChartController : MonoBehaviour {

    RawImage img;
    GameObject lastButton;
    
	/**
	*	Use this for initialization
	**/
    void Start () {
         img = GameObject.Find("img_flowchart").GetComponent<RawImage>();
        lastButton = GameObject.Find("btn_flowchartUeberblick");
    }
	
	/**
	*	Update is called once per frame
	**/
	void Update () {
		
	}
    
	/**
	*	flowchart: is called by a PointerClickEvent and change the image in P&I canvas
	*	@param pic: the new image
	**/
    public void flowchart(Texture pic)
    {
        
        img.texture = pic;
        //img.texture = Resources.Load("Giraffe_01") as Texture; 
    }

	/**
	*	disableButton: disable the current butto, the give a feedback, which one is choosen
	*	@param go: gameobject which has the button for disable
	**/
    public void disableButton(GameObject go)
    {
        lastButton.GetComponent<Button>().interactable = true;
        go.GetComponent<Button>().interactable = false;
        lastButton = go;
    }

}
