//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class PreloadBehaviour : MonoBehaviour
{
    
    private AsyncOperation async; //Prepare the async operations

    void Start()
    {
        async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); //Just load the next scene (the order of which is set in build settings.)
        async.allowSceneActivation = false; //Do not immediately change.
    }
    void Update()
    {
        if (async.progress >= 0.9f && SplashScreen.isFinished) //Check if both the loading and splash screens are finished.
        {
            async.allowSceneActivation = true;  //Then continue to next scene.
        }
    }
}
