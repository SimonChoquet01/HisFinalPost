//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    //--- Editor Parameters ---
    [SerializeField] string[] _messages; //Messages to pick from are set in the editor.
    [SerializeField] Text textBox; //The field they display to is also assigned in the editor.

    //--- Private Variables ---
    private AsyncOperation async;
    private bool ready = false;

    //--- Unity Methods ---
    void Start()
    {
        textBox.text = _messages[Random.Range(0, _messages.Length)];
        async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); 
        async.allowSceneActivation = false;
    }

    private void Update()
    {
        if (async.progress >= 0.9f && !ready)
        {
            ready = true;
            textBox.text += "\nPress any button to continue.";
        }
    }

    //--- Input Actions ---
    public void OnAny(InputAction.CallbackContext context)
    {
        if (ready)
        {
            async.allowSceneActivation = true;
        }
    }

}