using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusbarController : MonoBehaviour {
    // config
    [Header("Statusbar Settings")]
    [Tooltip("Sets the intervall in which current states are checked")]
    [Range(1.0f, 5.0f)]
    public float checkIntervall = 1.0f;
    [Tooltip("Prints all states to debug log if checked")]
    public bool logStates = false;
    [Tooltip("REST Controller to Check OPCUA Backend")]
    public GameObject restControllerObject = null;
    [Tooltip("Canvas for Display")]
    public Canvas statbarCanvas = null;

    // stati
    private bool internetReachable = false;
    string currentTime = "";
    float batteryLevel = 1;
    BatteryStatus batteryStatus = BatteryStatus.Unknown;
    bool endpointReachable = false;

    // helper
    private RESTController restController;

    Image batteryIndicator;
    Image internetIndicator;
    Image endpointIndicator;

    Text timeText;

    Sprite[] batterySprites = new Sprite[5];
    Sprite[] connectionSprites = new Sprite[2];
    Sprite[] endpointSprites = new Sprite[2];


    // Use this for initialization
    void Start () {
        // get all controller
        restController = restControllerObject.GetComponent<RESTController>();

        // get sprites
        batterySprites[0] = Resources.Load<Sprite>("Icons/bat_charging");
        batterySprites[1] = Resources.Load<Sprite>("Icons/bat_empty");
        batterySprites[2] = Resources.Load<Sprite>("Icons/battery-quarter");
        batterySprites[3] = Resources.Load<Sprite>("Icons/battery-half");
        batterySprites[4] = Resources.Load<Sprite>("Icons/bat_full");
        connectionSprites[0] = Resources.Load<Sprite>("Icons/net_err");
        connectionSprites[1] = Resources.Load<Sprite>("Icons/net_ok");
        endpointSprites[0] = Resources.Load<Sprite>("Icons/end_non");
        endpointSprites[1] = Resources.Load<Sprite>("Icons/end_ok");

        // find all elements
        batteryIndicator = statbarCanvas.transform.Find("BatteryIndicator").GetComponent<Image>();
        internetIndicator = statbarCanvas.transform.Find("InternetIndicator").GetComponent<Image>();
        endpointIndicator = statbarCanvas.transform.Find("EndpointIndicator").GetComponent<Image>();
        timeText = statbarCanvas.transform.Find("Time").GetComponent<Text>();

        // start state update routine
        InvokeRepeating("getStates", 0.0f, checkIntervall);
        InvokeRepeating("updateTime", 0.0f, 1);
    }

    #region internal functions
    // seperate time from rest, to not be affected by intervalls slower 1 second
    void updateTime()
    {
        currentTime = System.DateTime.UtcNow.ToString("r");
        timeText.text = currentTime;
    }


    void getStates()
    {
        internetReachable = (Application.internetReachability != NetworkReachability.NotReachable) ? true : false;
        batteryLevel = SystemInfo.batteryLevel;
        batteryStatus = SystemInfo.batteryStatus;

        restController.endpointStatus((reachable) =>
        {
            endpointReachable = reachable;
        });

        // log states (Async states may only available at next iteration)
        if (logStates) printStates();

        // show changes on statbar canvas
        if (statbarCanvas != null) updateDisplay();
    }

    void printStates()
    {
        Debug.Log("Time: " + currentTime 
            + "\nInternet: " + internetReachable.ToString() 
            + "\nBattery Status: " + batteryStatus.ToString()
            + "\nBattery Level: " + batteryLevel
            + "\nEndpoint:" + endpointReachable);
    }

    void updateDisplay()
    {

        // Set Battery Icon according to state and level
        if (batteryStatus == BatteryStatus.Charging)
        {
            batteryIndicator.sprite = batterySprites[0];
        } else
        {
            if (batteryLevel <= 0.1) {
                batteryIndicator.sprite = batterySprites[1];
            }
            else if (batteryLevel <= 0.4)
            {
                batteryIndicator.sprite = batterySprites[2];
            }
            else if (batteryLevel <= 0.8)
            {
                batteryIndicator.sprite = batterySprites[3];
            }
            else 
            {
                batteryIndicator.sprite = batterySprites[4];
            }
        }

        // Set Connection Indicator
        if (internetReachable)
        {
            internetIndicator.sprite = connectionSprites[1];
        } else
        {
            internetIndicator.sprite = connectionSprites[0];
        }

        // Set Endpoint Indicator
        if (endpointReachable)
        {
            endpointIndicator.sprite = endpointSprites[1];
        }
        else
        {
            endpointIndicator.sprite = endpointSprites[0];
        }

    }

    #endregion
}
