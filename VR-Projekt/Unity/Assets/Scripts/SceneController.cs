/*
*	author: XX , 
*	Script for ???
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Switch to scene
    public void ChangeScene(string scene)
    {
        // FADE OUT - but dont work! 
        //Animator animator;
       // animator = GameObject.Find("Fade").GetComponentInChildren<Animator>();
        //animator.SetBool("bool_fade_out", true);

      
        SceneManager.LoadScene(scene);
    
    }

    //Exit application
    public void CloseScene()
    {
        GvrDaydreamApi.LaunchVrHomeAsync((bool success) => {; });
    }



}
