/*
*	author: XX , 
*	Script for ???
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSettingsForAnlage : MonoBehaviour {
    public bool DontDestroyOnLoadboolean = true;
    public bool DontCreateNewWhenBackToThisScene = true;
    public static SaveSettingsForAnlage Instance = null;
    void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
        Instance = this;
        if (this.DontDestroyOnLoadboolean)
            GameObject.DontDestroyOnLoad(this);
    }
}
