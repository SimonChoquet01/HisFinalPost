//using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{

    //--- Editor Parameters ---
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Dropdown _qualityDropdown;
    [SerializeField] [Tooltip("First item selected from the main menu.")] private Selectable _firstMenuButton;
    [SerializeField] [Tooltip("First item selected from the options menu.")] private Selectable _firstOptions;

    //--- Unity Methods ---
    void Start()
    {
        //Set sliders
        _volumeSlider.value = PlayerPrefs.GetFloat("Vol", 0.0f);
        _sensitivitySlider.value = PlayerPrefs.GetFloat("Sens", 3.5f);
        //Populate dropdown
        _qualityDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (string str in QualitySettings.names)
        {
            options.Add(str);
        }
        _qualityDropdown.AddOptions(options);
        //Pick the one that's active (this is saved so no need for player prefs)
        _qualityDropdown.value = QualitySettings.GetQualityLevel();
        //Select the first button
        EventSystem.current.SetSelectedGameObject(_firstMenuButton.gameObject);
    }
    
    //---  UI Events ---
    public void OnQualityChanged()
    {
        QualitySettings.SetQualityLevel(_qualityDropdown.value);
    }

    public void OnVolAdjust()
    {
        PlayerPrefs.SetFloat("Vol", _volumeSlider.value);
    }

    public void OnSensAdjust()
    {
        PlayerPrefs.SetFloat("Sens", _sensitivitySlider.value);
    }
    public void OpenSettings()
    {
        Input.ResetInputAxes(); //No double press
        _mainMenu.SetActive(false);
        _optionMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_firstOptions.gameObject);
    }

    public void CloseSettings()
    {
        Input.ResetInputAxes(); //No double press
        _mainMenu.SetActive(true);
        _optionMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_firstMenuButton.gameObject);
    }
    public void OnPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnReset()
    {
        Input.ResetInputAxes(); //No double press
        PlayerPrefs.SetFloat("Vol", 0.0f);
        PlayerPrefs.SetFloat("Sens", 3.5f);
        PlayerPrefs.SetFloat("Score", 0.0f);
        CloseSettings();
        Start();
    }


    public void Exit()
    {
        PlayerPrefs.Save(); //Player prefs only save when the player properly chooses to quit the game. 
        //This second half of the function is not my code as mentioned earlier.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; //I just like being able to quit the test mode via a UI button ingame.
        #else
            Application.Quit();
        #endif
    }

}
