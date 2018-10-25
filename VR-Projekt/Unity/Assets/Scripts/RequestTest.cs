using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class RequestTest : MonoBehaviour
{

	public GameObject restControllerObject = null;
	public GameObject ldControllerObject = null;
	public bool makeTests = false;
	private RESTController restController;
	private LDController ldController;

    private bool lastValue = true;

	// Use this for initialization
	void Start ()
	{
		restController = restControllerObject.GetComponent<RESTController> ();
		ldController = ldControllerObject.GetComponent<LDController> ();


		// repeating test function
		if (makeTests) {
			InvokeRepeating ("TestREST", 2.0f, 3.0f);
			InvokeRepeating ("TestLD", 2.0f, 3.0f);
		}
	}

	void TestREST ()
	{
        restController.getDataByID("ns=3;i=17", (success, json) =>
        {

            if (success)
                Debug.Log(
                    "Rest Get Test: " +
                    success.ToString() + "\n" +
                    json.ToString());
            else
                Debug.Log("Rest Get Test: false");
        });

        restController.setDataByID("ns=3;i=17", opcuaDataType.Boolean, lastValue.ToString(), (success, response) => {

            if (success)
                Debug.Log(
                    "Rest Post Test: " +
                    success.ToString() + "\n" +
                    response);
            else
                Debug.Log("Rest Post Test: false");
        });

        // change bool to opposite value -> everchanging value for tests
        lastValue = (lastValue) ? false : true;
    }

	void TestLD ()
	{
		ldController.getElementData ("=0.H1.T1.RI1-H004", (success, json) => {
			if (success)
				Debug.Log (
					"LD Test: " +
					success.ToString () + "\n" +
					json.ToString ());
			else
				Debug.Log ("LD Test: false");
		});
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}
