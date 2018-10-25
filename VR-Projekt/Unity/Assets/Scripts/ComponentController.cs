/*
*	author: Maximilian Stecklina, Markus Rudolph
*	Script to control component canvas:
*		- position
*		- show values
*		- interaction
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]
public class ComponentController : MonoBehaviour
{
	//private var
	private GameObject selectedObject;
	private ArrayList aciveComponentList = new ArrayList ();
	private float numberOfPages = 0;
	private int page_counter = 0;
	private int current_page = 1;
	private int toggle_count = 0;

	//public var
	public Camera MainCamera;

	// sounds
	private AudioSource audioSource;
	private AudioClip sndHover;
	private AudioClip sndClick;

	/*
	 *	Use this for initialization
	 */
	void Start ()
	{
		// get audiosource for playback
		GameObject go_AudioSource = GameObject.Find ("Audio");
		audioSource = go_AudioSource.GetComponent<AudioSource> ();

		// load sounds
		sndHover = Resources.Load<AudioClip> ("Sounds/hover");
		sndClick = Resources.Load<AudioClip> ("Sounds/ClickUI");
	}

	/**
	*	Update is called once per frame
	**/
	void Update ()
	{
		if (selectedObject != null) {
			showOpcValue ();
		}
	}

	/**
	*	showComponentDetails open an canvas with all relevent information for an component
	*	calculate the rotation of the canvas to the poistion of the user
	**/
	private void showComponentDetails ()
	{
        

		show_canvas_OPC (); // show canvas for OPC-UA live data and hide LD
		showOpcValue ();

		//get Pose of Camera
		Vector3 MainCamPos = MainCamera.transform.position;
		Quaternion MainCamRot = MainCamera.transform.rotation;
		Vector3 orientation = MainCamRot.eulerAngles;

		//Debug.Log("camRot" + MainCamRot);
		//Debug.Log("camPos" + MainCamPos);

		//get Pose of Component
		Vector3 selectedCompPos = selectedObject.transform.position;
		Quaternion selectedCompRot = selectedObject.transform.rotation;
        
		//Debug.Log("compRot" + selectedCompRot);
		//Debug.Log("compRos " + selectedCompPos);

		//calculate Pose of Canvas
		Vector3 CanPos;
		Quaternion CanRot;

		CanPos = selectedObject.GetComponent<Collider> ().ClosestPointOnBounds (MainCamPos);
		CanRot = Quaternion.Euler (orientation.x, orientation.y, 0);

		//Debug.Log("can pos" + leftCanPos);
		//Debug.Log("can rot" + leftCanRot);

		Canvas compCanvas = GameObject.Find ("compCanvas").GetComponent<Canvas> ();

		//Debug.Log("comp can " + compCanvas);

		compCanvas.transform.SetPositionAndRotation (CanPos, CanRot);
		compCanvas.enabled = true;

		//creat Canvas left (for interaction)

		// enable Box-Collider from Canvas
		// btn_OPC & btn_LD
		able_BoxCollider (GameObject.Find ("btn_OPC"), true);
		able_BoxCollider (GameObject.Find ("btn_LD"), true);
		// left & right Canvas
		able_BoxCollider (GameObject.Find ("leftCanvas"), true);
		able_BoxCollider (GameObject.Find ("rightCanvas"), true);

		//btn OPC set value
		able_BoxCollider (GameObject.Find ("btn_switch"), true);



		// show interaction if the component is from type pump or ventil
		string state = "";
		string action = "";
		bool btn_interactable;

		if (selectedObject.name.Contains ("pump")) {

			if (component_active ()) {
				state = "eingeschaltet";
				action = "ausschalten";
			} else {

				state = "ausgeschaltet";
				action = "einschalten";
                
			}

			state = "Die Pumpe ist " + state;
			btn_interactable = true;

		} else if (selectedObject.name.Contains ("ventil") || selectedObject.name.Contains ("Pneumatisch") || selectedObject.name.Contains ("Regelve")) {
			if (component_active ()) {
				state = "geöffnet";
				action = "schließen";
			} else {

				state = "geschlossen";
				action = "öffnen";
                
			}

			state = "Das Ventil ist " + state;
			btn_interactable = true;

		} else {
			state = "keine Interaktion\nmöglich\nsorry :/";
			action = "-";
			btn_interactable = false;
		}
		GameObject.Find ("text_pump").GetComponent<Text> ().text = state;
		GameObject.Find ("btn_switch").GetComponent<Button> ().interactable = btn_interactable;
		GameObject.Find ("btn_switch").GetComponentInChildren<Text> ().text = action;

	}


	// SET and GET selected Object
	
	/**
	*	setSelectedGameObject: save the last/current choosen component
	* 	@param new_selected: require the gameobject who called this function over an PointerClickEvent
	**/
	public void setSelectedGameObject (GameObject new_selected)
	{
		// do cracy stuff with old selected Object
		if (selectedObject != null) {//&& new_selected != selectedObject)
			//Debug.Log("es war bereits ein Objekt ausgewählt");
            
			//change material to normal (deselect / old object)
			
			selectedObject.GetComponent<Renderer> ().material = selectedObject.GetComponent<ComponentColor> ().getNormalMaterial ();
			if (new_selected == null)
				selectedObject = null;
		}

		if (new_selected != null) {
			//change material to select (select / new object)
			new_selected.GetComponent<Renderer> ().material = new_selected.GetComponent<ComponentColor> ().getSelectedMaterial ();

			//close old Canvas
			CloseCompCanvas ();

			//set new selected Object
			selectedObject = new_selected;

			//show new Canvas
			showComponentDetails ();
		}
	}

	/**
	*	get SelectedGameObject from the script
	* 	@return GameObject selectedObject
	**/
	public GameObject getSelectedGameObject ()
	{
		return selectedObject;
	}

	/**
	*	DestroySelectedGameObject: sets the current selected object to "null"
	**/
	public void DestroySelectedGameObject ()
	{
		Debug.Log ("selectedObject wird gelöscht");
		selectedObject = null;
	}

	/**
	*	EnterHoverEffect: use this with PointerEnterEvent from a component to start a hover effect
	*	@param obj: the gameobject which call this function
	**/
	public void EnterHoverEffect (GameObject obj)
	{
		//Debug.Log("selectedObject: " + selectedObject);
		//Debug.Log("obj: " + obj);
		if (selectedObject != null) {
			if (selectedObject == obj) {
				//Debug.Log("obj: " + obj + " ===== selected: " + selectedObject);
				//Debug.Log("Hover wird nicht ausgeführt");
				return;
			} else
				obj.GetComponent<Renderer> ().material = obj.GetComponent<ComponentColor> ().getHoveringMaterial ();
		} else {
			obj.GetComponent<Renderer> ().material = obj.GetComponent<ComponentColor> ().getHoveringMaterial ();
			//Debug.Log("Hover - Exit - ChangeMaterial");

		}

		// change material to select (select / new object)
		//new_selected.GetComponent<Renderer>().material = new_selected.GetComponent<ComponentColor>().getSelectedMaterial();

		// Play Sound
		audioSource.PlayOneShot (sndHover);

	}

	/**
	*	EnterHoverEffect: use this with PointerExitEvent from a component to end a hover effect
	*	@param obj: the gameobject which call this function
	**/
	public void ExitHoverEffect (GameObject obj)
	{
		//Debug.Log("selectedObject: " + selectedObject);
		if (selectedObject != null) {
			if (selectedObject == obj) {
				//Debug.Log("obj: " + obj + " ===== selected: " + selectedObject);
				//Debug.Log("Hover wird nicht ausgeführt");
				return;
			} else
				obj.GetComponent<Renderer> ().material = obj.GetComponent<ComponentColor> ().getNormalMaterial ();
		} else {
			obj.GetComponent<Renderer> ().material = obj.GetComponent<ComponentColor> ().getNormalMaterial ();
			//Debug.Log("Hover - Exit - ChangeMaterial");
		}
	}

	/**
	*	CloseCompCanvas: close the canvas and disablle all boxcollider
	**/
	public void CloseCompCanvas ()
	{
		
		//Debug.Log("CloseCompCanvas();");

		//GameObject.Find("compCanvas").GetComponent<ComponentController>().DestroySelectedGameObject();
		//DestroySelectedGameObject();
		setSelectedGameObject (null);
		//Debug.Log("Canvas enablen");
		GameObject.Find ("compCanvas").GetComponent<Canvas> ().enabled = false;


		// disable Boxcollider 
		//btn_OPC & btn_LD
		able_BoxCollider (GameObject.Find ("btn_OPC"), false);
		able_BoxCollider (GameObject.Find ("btn_LD"), false);
		// left & right canvas
		able_BoxCollider (GameObject.Find ("leftCanvas"), false);
		able_BoxCollider (GameObject.Find ("rightCanvas"), false);

		// disable all OPC entries
		GameObject canOPC = GameObject.Find ("canvas_OPC");
		foreach (Canvas can in canOPC.GetComponentsInChildren<Canvas>()) {
			can.enabled = false;
		}

		//set counter from LD and dict_counter to zero
		page_counter = 0;
		current_page = 1;

		//LD next - prev button: disable boxcollider
		able_BoxCollider (GameObject.Find ("btn_next"), false);
		able_BoxCollider (GameObject.Find ("btn_prev"), false);
		able_BoxCollider (GameObject.Find ("canvas_LD"), false);

		//OPC set value
		able_BoxCollider (GameObject.Find ("btn_switch"), false);

	}


	#region all to update canvas informations

	/**
	*	showOpcValue: update the opc ua values on the canvas
	**/
	private void showOpcValue ()
	{
		GameObject canOPC = GameObject.Find ("canvas_OPC");
        
		int i = 0;

		//get all dictionarys from component and all subcomponent
		foreach (DataMemory entry in selectedObject.GetComponentsInChildren<DataMemory>()) {

			//get dictionary from component
			Dictionary<string, string> local_dic = new Dictionary<string, string> ();
			local_dic = entry.GetComponent<DataMemory> ().getOPCValue ();

			GameObject el = canOPC.transform.Find ("element" + i).gameObject;

			//set attribut
			GameObject attr = el.transform.Find ("attribut").gameObject;
			Text attrText = attr.GetComponent<Text> ();
			try {
				attrText.text = local_dic ["Displayname"];
			} catch {
				Debug.Log ("Error with Displayname");
			}

			//set value
			GameObject val = el.transform.Find ("value").gameObject;
			Text valText = val.GetComponent<Text> ();
			try {
				valText.text = local_dic ["Value"];
			} catch {
				Debug.Log ("Error with Value");
			}

			el.GetComponent<Canvas> ().enabled = true;           
			//Debug.Log (entry + " -> " + attrText.text + ": " + valText.text);
			i++;
		}


	}

	/**
	*	showLD: update the linked data information with content from the selected object
	**/
	private void showLD ()
	{
		//get all dataMemorys
		int numberOfEntry = 0;
		numberOfPages = 0;

		foreach (DataMemory entry in selectedObject.GetComponentsInChildren<DataMemory>()) {
			//get dict
			Dictionary<string, string> local_dic = new Dictionary<string, string> ();
			local_dic = entry.GetComponent<DataMemory> ().getLdValues ();
			//count the total number of entries
			numberOfEntry += local_dic.Count;
			//calculate the number of pages, which needed to show all the content
			numberOfPages += Mathf.Ceil ((float)local_dic.Count / 5);

			//Debug.Log("numPages: " + numberOfPages);
		}

		string[,] dataArray = new string[numberOfEntry, 3];

		// get all linked Data KEY:VALUE pairs and write to dataArray
		int i = 0;
		foreach (DataMemory datmem in selectedObject.GetComponentsInChildren<DataMemory>()) {
			Dictionary<string, string> dict = datmem.getLdValues ();
			foreach (KeyValuePair<string, string> entry in dict) {
				dataArray [i, 0] = datmem.name; // name der Komponente! 
				dataArray [i, 1] = entry.Key;
				dataArray [i, 2] = entry.Value;
				i++;
			}
		}

		//write data to TEXT elements depending on the current page_counter
		GameObject canLD = GameObject.Find ("canvas_LD");
		canLD.transform.Find ("Headline").gameObject.GetComponent<Text> ().text = dataArray [0, 0] + " and children";
		for (int j = 0; j <= 4; j++) {
			GameObject el = canLD.transform.Find ("element" + j).gameObject;
			//set attribut
			GameObject attr = el.transform.Find ("attribut").gameObject;
			Text attrText = attr.GetComponent<Text> ();
			try {
				attrText.text = dataArray [page_counter + j, 1] + ":";
			} catch {
				attrText.text = "keine";
				//Debug.Log("Error with LD attribut");
			}
			//set value
			GameObject val = el.transform.Find ("value").gameObject;
			Text valText = val.GetComponent<Text> ();
			try {
				valText.text = dataArray [page_counter + j, 2];
			} catch {
				valText.text = "Daten";
				//Debug.Log("Error with LD value");
			}
		}

		//set page number
		canLD.transform.Find ("page_counter").gameObject.GetComponent<Text> ().text = ((page_counter / 5) + 1) + " von " + numberOfPages;

		//Disable btn_next if only one page
		if (numberOfPages == 1)
			GameObject.Find ("btn_next").GetComponent<Button> ().interactable = false;
		if (numberOfPages >= 2 && current_page != numberOfPages)
			GameObject.Find ("btn_next").GetComponent<Button> ().interactable = true;


	}

	/**
	*	nextLDPage increase the page number to show the next five elements at linked data
	**/
	public void nextLDPage ()
	{
		bool btn_active = true;
		if (current_page < numberOfPages) {
			page_counter += 5;
			current_page++;
			GameObject.Find ("btn_prev").GetComponent<Button> ().interactable = true;
			if (current_page == numberOfPages)
				btn_active = false;
		} else {
			btn_active = false;
		}

		GameObject.Find ("btn_next").GetComponent<Button> ().interactable = btn_active;

	
		showLD ();
	}

	/**
	*	previousLDPage decrease the page number to show the last five elements at linked data
	**/
	public void previousLDPage ()
	{
		bool btn_active = true;
		if (current_page > 1) {
			page_counter -= 5;
			current_page--;
			GameObject.Find ("btn_next").GetComponent<Button> ().interactable = true;
			if (current_page == 1)
				btn_active = false;
		} else {
			btn_active = false;
		}

		GameObject.Find ("btn_prev").GetComponent<Button> ().interactable = btn_active;

		showLD ();
	}

	/**
	*	toggle the interaction option of the current component between the two states
	**/	
	public void toggle ()
	{
		string action = "";
		string state = "";
		string type = "";

		if (selectedObject.name.Contains ("ventil") || selectedObject.name.Contains ("Pneumatisch") || selectedObject.name.Contains ("Regelve"))
			type = "ventil";
		else if (selectedObject.name.Contains ("pump"))
			type = "pump";
		else
			type = "other";


			//Debug.Log ("Name: " + selectedObject.name);
			//Debug.Log ("Type: " + type);
			//Debug.Log ("bool: " + component_active ());
	
		if (component_active ()) { // Component WAS active
				//Debug.Log ("Komponente WAR aktiv");
			// change text for btn-text and state 
			if (type == "ventil") {
				state = "Das Ventil ist geschlossen";
				action = "öffnen";
			} else if (type == "pump") {
				state = "Die Pumpe ist ausgeschalten";
				action = "einschalten";
			} else {
				//Debug.Log ("Shit :/");
			}

			// remove selecetedObject from selectedObjectList
			removeActiveComponentFromList ();
		} else {// component wasn't acitve
				//Debug.Log ("Komponente WAR NICHT aktiv");

			// change text for btn-text and state 
			if (type == "ventil") {
				state = "Das Ventil ist geöffnet";
				action = "schließen";
			} else if (type == "pump") {
				state = "Die Pumpe ist eingeschalten";
				action = "ausschalten";
			} else {
				//Debug.Log ("Shit :(");
			}

			// add selecetedObject to activeComponentList
			addActiveComponent2List ();
		}

		// update text-labels in object/ canvas
		GameObject.Find ("text_pump").GetComponent<Text> ().text = state;
		GameObject.Find ("btn_switch").GetComponentInChildren<Text> ().text = action;
	}


	private void addActiveComponent2List ()
	{
		aciveComponentList.Add (selectedObject.name);
	}

	private void removeActiveComponentFromList ()
	{
		aciveComponentList.Remove (selectedObject.name);
	}

	/**
     * check if selectedObject is active (pump and ventil) 
     */
	private bool component_active ()
	{
		bool is_active = false;

		if (aciveComponentList.Contains (selectedObject.name))
			is_active = true;
        
		return is_active;
	}


	#endregion

	/**
	*	show_canvas_OPC ?
	**/
	public void show_canvas_OPC ()
	{
		//Debug.Log("OPC true ... LD false");
		//CANVAS
		GameObject.Find ("canvas_OPC").GetComponent<Canvas> ().enabled = true;
		GameObject.Find ("canvas_LD").GetComponent<Canvas> ().enabled = false;

		//BUTTONS
		// interactible
		GameObject.Find ("btn_OPC").GetComponent<Button> ().interactable = false;
		GameObject.Find ("btn_LD").GetComponent<Button> ().interactable = true;

		//LD next - prev button: disable boxcollider
		able_BoxCollider (GameObject.Find ("btn_next"), false);
		able_BoxCollider (GameObject.Find ("btn_prev"), false);
		able_BoxCollider (GameObject.Find ("canvas_LD"), false);

		showOpcValue ();
	}

	/**
	*	show_canvas_LD: show the linked data canvas and enable all boxcollider 
	**/
	public void show_canvas_LD ()
	{
		//Debug.Log("OPC false ... LD true");
		//CANVAS
		GameObject.Find ("canvas_LD").GetComponent<Canvas> ().enabled = true;
		GameObject.Find ("canvas_OPC").GetComponent<Canvas> ().enabled = false;

		//BUTTONS
		GameObject.Find ("btn_OPC").GetComponent<Button> ().interactable = true;
		GameObject.Find ("btn_LD").GetComponent<Button> ().interactable = false;

		//LD next - prev button: enable boxcollider
		able_BoxCollider (GameObject.Find ("btn_next"), true);
		able_BoxCollider (GameObject.Find ("btn_prev"), true);
		able_BoxCollider (GameObject.Find ("canvas_LD"), true);

		//update informations on canvas
		showLD ();
	}

	/**
	*	able_BoxCollider: help function to disable/enable boxcolloder in the gameobject and all childre
	* 	@param obj: the start gameobject
	*	@param enable: true/false to enable/disable
	**/
	private void able_BoxCollider (GameObject obj, bool enable)
	{
		BoxCollider[] boxes = obj.GetComponentsInChildren<BoxCollider> ();
		for (var i = 0; i < boxes.Length; i++) {
			boxes [i].enabled = enable;
		}
	}

}
	