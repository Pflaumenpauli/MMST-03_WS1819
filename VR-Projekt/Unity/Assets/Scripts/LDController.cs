/*
*	author: XX , 
*	Script for ???
*/
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class LDController : MonoBehaviour {

    // setable variables
    public GameObject mainControllerObject = null;
    public GameObject restControllerObject = null;


    // private variables
    private MainController mainController;
    private RESTController restController;

    private String sparqlEndpoint;
    private String dataGraph;

	private UnityEngine.Object[] requestFiles;
	private Dictionary<String, String> requests = new Dictionary<String, String>();

    #region Unity Methods

    /**
	*	Use this for initialization
	**/
    void Awake () {
        if ((mainControllerObject != null) && (restControllerObject != null))
        {
            mainController = mainControllerObject.GetComponent<MainController>();
            restController = restControllerObject.GetComponent<RESTController>();

            sparqlEndpoint = mainController.getSPARQLEndoint();
            dataGraph = mainController.getDataGraph();
        }

		// load sparql filesochmal klappen (:

		requestFiles = Resources.LoadAll("Sparql/", typeof(TextAsset));
		foreach (TextAsset s in requestFiles)
		{
			requests.Add(s.name, s.text);
		}
		requestFiles = null;
        Debug.Log("starte der LD controller");
    }
    #endregion


    #region Public Methods for Data Access
	/**
	* 	getElementData ??
	*	@param	identifier ?? 
	*	@param	callback ??
	**/
    public void getElementData(string identifier, Action<bool, JObject> callback)
    {
        foreach(KeyValuePair<string,string> entry in requests)
        {
            Debug.Log("key: " + entry.Key + "  value: " + entry.Value);
        }
        // TODO: Create sparql file loader
        string query = requests["getComponent"];
        Dictionary<string, string> placeHolder = new Dictionary<string, string>();
        placeHolder.Add("LD_GRAPH", dataGraph);
        placeHolder.Add("LD_IDENTIFIER", identifier);

        query = prepareRequest(query, placeHolder);


        // setting up parameter
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter.Add("default-graph-ur", dataGraph);
        parameter.Add("query", query);
        parameter.Add("format", "application/sparql-results+json");
        parameter.Add("timeout", "5");

        if (restController != null) {
            restController.Get(sparqlEndpoint, parameter, true, (success, response) =>
            {
                JObject parsedResult;

                if (success)
                {
                    try
                    {
                        parsedResult = JObject.Parse(response);
                        callback(true, parsedResult);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Exception while parsing response: \n" + e.Message);
                    }
                } else
                {
                    callback(false, null);
                }


            });

        } else
        {
            Debug.Log("No Reference to REST-Controller found...");
            callback(false, null);
        }
    }

    #endregion

    #region Private Methods of Class
	/**
	* 	prepareRequest ??
	*	@param	identifier ?? 
	*	@param	callback ??
	*	@return	string rawRequest ??
	**/
    private string prepareRequest(string rawRequest, Dictionary<String, String> placeholder)
    {
        string pattern = @"\$\$(\S+)\$\$";

        foreach (Match m in Regex.Matches(rawRequest, pattern))
        {
            if (placeholder.ContainsKey(m.Groups[1].ToString()))
            {
                rawRequest = rawRequest.Replace(m.Value, placeholder[m.Groups[1].ToString()]);
            }
            else
            {
                Debug.LogError("Unknown Placeholder: " + m.Groups[1]);
            }
        }
        Debug.Log(rawRequest);
        return rawRequest;
    }

    #endregion

}
