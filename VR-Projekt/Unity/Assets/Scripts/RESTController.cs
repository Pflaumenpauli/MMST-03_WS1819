using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

using Newtonsoft.Json.Linq;

public enum opcuaDataType {
    Null,
    Boolean,
    SByte,
    Byte,
    Int16,
    UInt16,
    UInt32,
    Int64,
    Uint64,
    Float,
    Double,
    String,
    DataTime,
    Guid,
    ByteString,
    XmlElement,
    NodeId,
    ExpandedNodeId,
    StatusCode,
    QualifiedName,
    LocalizedText,
    ExtensionObject,
    DataValue,
    Variant,
    DiagnosticInfo
}

public class RESTController : MonoBehaviour
{

    // setable variables
    public GameObject mainControllerObject = null;
    public int timeout = 0;

    // private variables
    private MainController mainController;
    private string restEndpoint;


    #region Unity Methods

    // Use this for initialization
    void Awake()
    {
        if (mainControllerObject != null)
        {
            mainController = mainControllerObject.GetComponent<MainController>();
            restEndpoint = mainController.getRestEndpoint();
        }
    }

    void Update()
    {
    }
    #endregion


    #region public methods for gameobjects
    public void getDataByID(string id, Action<bool, JObject> callback)
    {
        // parameter
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter.Add("nodeId", id);

        // start request
        Get(restEndpoint + "/base/getNode" , parameter, true, (success, resText) =>
        {
            JObject parsedResult = null;

            // try to parse result to json object
			if (success && string.IsNullOrEmpty(resText)) {
	            try
	            {
					
	                parsedResult = JObject.Parse(resText);
	            }
	            catch (Exception e)
	            {
	                Debug.Log("Exception while parsing response: \n" + e.Message);
	            }
            }

            // return information to callback
            if (parsedResult != null)
            {
                callback(true, parsedResult);
            }
            else
            {
                callback(false, parsedResult);
            }
        });
    }

    public void setDataByID(string id, opcuaDataType dataType, string value, Action<bool, String> callback)
    {
        // parameter
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter.Add("nodeId", id);
        parameter.Add("datatype", dataType.ToString());
        parameter.Add("value", value);

        Post(restEndpoint + "/base/writeValue", parameter, (success, response) =>
        {
            // no work to do in this case. response is expected to be string
            callback(success, response);
        });
    }

    public void endpointStatus(Action<bool> callback)
    {
        // start request
        Get(restEndpoint, null, false, (success, resText) =>
        {
            callback(success);
        });
    }
    #endregion

    #region Request Methods and Wrappers
    public Coroutine Get(string url, Dictionary<string, string> parameter, bool isQuery, Action<bool, string> callback)
    {
        UnityWebRequest request;

        if (isQuery)
        {
            // url encode parameters
           string parameterString = Encoding.UTF8.GetString(UnityWebRequest.SerializeSimpleForm(parameter));
            request = UnityWebRequest.Get(url + "?" + parameterString);
        }
        else
        {
            // put parameters 
            request = UnityWebRequest.Get(url);
            if (parameter != null)
            {
                foreach (KeyValuePair<String, String> par in parameter)
                {
                    request.SetRequestHeader(par.Key, par.Value);
                }
            }
        }
        return this.StartCoroutine(makeRequest(request, callback));
    }

    public Coroutine Post(string url, Dictionary<string, string> parameter, Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> par in parameter)
        {
            form.AddField(par.Key, par.Value);
        }

        WWW request = new WWW(url, form);

        return this.StartCoroutine(makeRequest(request, callback));
    }

    private IEnumerator makeRequest(UnityWebRequest request, Action<bool, string> callback)
    {

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("HTTP Error:\n" + request.error + "\nCode:\n" + request.responseCode + "\nText:\n" + request.downloadHandler.text);
            callback(false, request.error);
        }
        else
        {
            callback(true, request.downloadHandler.text);
        }
    }

    // Routine to execute WWW Requests. -> Fallback for Endpoints which don't accept UnityWebRequest
    private IEnumerator makeRequest(WWW request, Action<bool, string> callback)
    {
        yield return request;
        if(request.isDone)
        {
            Debug.LogWarning(request.responseHeaders);
            callback(true, request.text);
        }
        else if (!string.IsNullOrEmpty(request.error))
        {
            Debug.Log("HTTP Error:\n" + request.error + "\nResponse Text:\n" + request.text);

        }
    }

    #endregion

}
