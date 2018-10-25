/*
*	author: XX , 
*	Script for ???
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

    // Public setable configs
    [Header("REST Settings")]
    public string restEndpoint = "http://localhost";

    [Header("SPARQL Settings")]
    public string sparqlEndpoint = "http://localhost";
    public string dataGraph = "http://example.org/example";

    /**
	*	Use this for initialization
	**/
    void Start()
    {

    }


    /**
	*	Update is called once per frame
	**/
    void Update () {
		
	}

    // setter and getter
	
	/**
	*	getRestEndpoint give the restEndpoint back
	*	@return String restEndpoint url
	*/
    public string getRestEndpoint() {
        return restEndpoint;
    }

	/**
	*	getSPARQLEndoint give the getSPARQLEndoint back
	*	@return String sparqlEndpoint url
	*/
    public string getSPARQLEndoint()
    {
        return sparqlEndpoint;
    }

	/**
	*	getDataGraph give the restEndpoint back
	*	@return String dataGraph
	*/
    public string getDataGraph()
    {
        return dataGraph;
    }

}
