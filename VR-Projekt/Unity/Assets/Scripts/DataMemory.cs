/*
*	author: Markus Rudolph, 
*	Script which hold the hole data for one component (this one who hold this script)
*/


using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using Newtonsoft.Json.Linq; 
using System;

public class DataMemory : MonoBehaviour { 

	//setable variables 
	[Header("Linked Data Endpoint")] 
	[Tooltip("Linked-Data-Identifier of the Objekt" + 
		"e.g. '=0.H1.T3.RI1-H001' ")] 
	public string plantID; 
	[Header("OPC UA Server")]  
	[Tooltip("Both are only numbers!")] 
	public int Namespace = 3; 
	[Tooltip("Both are only numbers!")] 
	public int NodeID = -1; 
	[Header("")]
	[Tooltip("Enable for more debug logs")]
	public Boolean debug = false;



	//private variables 
	private RESTController restController;
	private LDController ldController;

	private JObject LDJson; 
	private string fullNodeID; 
	private JObject OPCJson;
	private Dictionary<string, string> opcData = new Dictionary<string, string>();
	private Dictionary<string , string> ldData  = new Dictionary<string, string>();
	private int count;

	#region Unity Methods
	/**
	* Use this for initialization 
	**/
	void Start () { 
		restController = GameObject.Find ("RESTController").GetComponent<RESTController> ();
		ldController = GameObject.Find ("LDController").GetComponent<LDController> ();

		fullNodeID = "ns=" + Namespace.ToString() + ";i=" + NodeID.ToString(); 
		if(debug) Debug.Log ("Full NodeID of " + this.name + " ist '" + fullNodeID+"'");


		if(debug) Debug.Log ("now get ld data of " + plantID);

		if ( plantID != "") {
			grabLDValue ();
		}
	} 


	/**
	*	Update is called once per frame 
	**/
	void Update () { 
		if (NodeID != -1){		
			// after 50 frames
			if (count < 50) {
				count++;
			} else {
				if(debug) Debug.Log ("now get opc values of " + fullNodeID);

				grabOpcValue ();

				count = 0;
			}
		}

	} 

	#endregion Unity Methods
	
	#region OPC UA Value -> Dictionary
	//OPC
	/**
	*	grabOpcValue get the opc ua data over the restController, give a callback for saveRestData
	**/
	private void grabOpcValue(){ 
		restController.getDataByID(fullNodeID,saveRestData);
	} 

	/**
	*	saveRestData is a callback from the restController with 
	*	all opc ua information
	*	@param success boolean for get a result from the opc ua server
	*	@param the response opc ua data
	**/
	private void saveRestData(bool success, JObject data){
		if (success) {
			OPCJson = data;
			if(debug) Debug.Log ("OPC - Daten erfolgreich abgelegt");
			if(debug) Debug.Log (OPCJson);
            
            //clear DICT here
            opcData.Clear();

			//NodeID
			string node = OPCJson["NodeId"].ToString();
			opcData.Add ("NodeID", node);

			//Browsename
			string brwName = OPCJson["BrowseName"]["name"].ToString() ;
			opcData.Add ("Browsename", brwName);

			//Description
			string descr = OPCJson["Description"]["text"].ToString();
			opcData.Add ("Description", descr);

			//Displayname
			string dispName = OPCJson["DisplayName"]["text"].ToString();
			opcData.Add ("Displayname", dispName);

			//Value
			string value = OPCJson["Value"].ToString();
			opcData.Add ("Value", value);

			//DataType
			string dataTypeNode = OPCJson["DataType"].ToString();
			restController.getDataByID (dataTypeNode,  addDataType);


		} else {
			if (debug) Debug.Log ("keine Erfolg mit OPC - Daten: success= " + success);
		}

		if(debug) {
			foreach(KeyValuePair<string, string> entry in opcData)
			{
				if (debug) Debug.Log ("opc" + entry.Key + " - " + entry.Value);
			}
		}
	}

	/**
	*	addDataType: second request to get the data type of the value
	*	@param success: true if request success
	*	@param json: response data
	**/
	private void addDataType (Boolean success, JObject json){
		if (success) {
			string dataType = json ["DisplayName"] ["text"].ToString ();
			opcData.Add ("DataType", dataType);
		}
	}

	#endregion


	#region LD Nodes -> Dictionary

	/**
	* 	grabLDValue: get linked data from linked data controller, use saceLDData as callback
	**/
	private void grabLDValue(){
		ldController.getElementData (plantID, saveLDData);
	}

	/**
	*	saveLDData: callback for grabLDValue, save the linked data to an dictionary
	*	@param 	success: true if request success
	*	@param 	data: response data
	**/
	private void saveLDData(bool success, JObject data){
		if (success) {
			LDJson = data;
			if(debug) Debug.Log ("LD - Daten erfolgreich abgelegt");
			if(debug) Debug.Log (LDJson);

			//save all elements in dictionary
			foreach (JObject item in LDJson["results"]["bindings"]){
				string  att = item["attributeSTR"]["value"].ToString() ;
				string val =item["valueSTR"]["value"].ToString() ;
				ldData.Add(att,val);
			}

			if(debug) Debug.Log ("elemente in dict von "+this.name+": "+ldData.Count);

		} else {
			if (debug) Debug.Log ("keine Erfolg: success= " + success);
		}


		if(debug){
			foreach(KeyValuePair<string, string> entry in ldData)
			{
				if(debug) Debug.Log ("LD:" + entry.Key + " - " + entry.Value);
			}
		}
		
	}

	#endregion



	#region getter (and setter - not yet)

	/**
	*	getLdValues give the Linked Data information back
	* 	@return Dictionary<string,string> ldData - LD  in dictionary 	Debug.Log ("att: " + att + " val: " + val);
	**/
	public Dictionary<string,string> getLdValues() { 
		return ldData;  
	} 

	/**
	*	getOPCValue give the opc ua information back
	* 	@return Dictionary<string,string> opcData  in dictionary 	Debug.Log ("att: " + att + " val: " + val);
	**/
	public Dictionary<string,string> getOPCValue (){
		return opcData;
	}

	#endregion
} 
