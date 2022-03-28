//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoadBehaviour : MonoBehaviour
{

    //--- Private Variables ---
    private AsyncOperation async;
    private AudioSource audioOut;

    //--- Unity Methods ---
    void Start()
    {
        audioOut = this.GetComponent<AudioSource>();
        async = SceneManager.LoadSceneAsync("MainMenu");
        async.allowSceneActivation = false; //Do not immediately change.
        audioOut.outputAudioMixerGroup.audioMixer.SetFloat("Vol", PlayerPrefs.GetFloat("Vol",0.0f)); //Load previously set setting
    }

    //--- Animator Events ---
    public void AllowNextScene() //Called by anim
    {
        async.allowSceneActivation = true; //Will move to loaded scene once done loading it.
    }

    public void PlayFirstSound() //Called by anim
    {
        audioOut.PlayOneShot(audioOut.clip); //Could've used a serialized audioClip if the cut scene had multiple sounds
    }

    public void PlaySecondSound() //Called by anim
    {
        audioOut.pitch = 0.5f; //This one needs to be lower (Sounds cooler)
        audioOut.PlayOneShot(audioOut.clip); //Could've used a serialized audioClip if the cut scene had multiple sounds
    }

}
