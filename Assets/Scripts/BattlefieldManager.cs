//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

//Handles keeping the player on the battle field.
public class BattlefieldManager : MonoBehaviour
{
    //--- Editor Parameters ---
    [SerializeField][Tooltip("The player's game object.")] private GameObject _player;
    [SerializeField][Tooltip("The text that appears once out of the battlefield.")] private GameObject _returnText;

    //--- Private Variables ---
    private bool _outOfBounds = false;
    private float _timeSinceLeft = 0.0f; //Count how long the player has been out of bounds.
    private bool _isInHouse = true; //To verify if the player is in the safe house.

    //For the collider of the house.
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameManager.Instance.Started)
            {
                _outOfBounds = true;
                _timeSinceLeft = 0.0f;
            }
            _isInHouse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameManager.Instance.Started)
            {
                _outOfBounds = false;
                _timeSinceLeft = 0.0f;
            }
            else
            {
                GameManager.Instance.StartGame();
            }
            _isInHouse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Handle radius
        if (!_isInHouse)
        {
            if (_player.transform.position.magnitude > 100)
            {
                _outOfBounds = true;
            }
            else if (_player.transform.position.magnitude < 100)
            {
                _outOfBounds = false;
                _timeSinceLeft = 0.0f;
            }
        }
        //Handle warning and gameOver
        if (!GameManager.Instance.Ended) {
            if (_outOfBounds)
            {
                _timeSinceLeft += Time.deltaTime;
                _returnText.SetActive(true);
                _returnText.GetComponent<Text>().text = "Return To Battlefield!\n" + (5 - (int)_timeSinceLeft);
            }
            else
            {
                _returnText.SetActive(false);
            }
        }
        else
        {
            _returnText.SetActive(false);
        }
        if (_timeSinceLeft > 5)
        {
            GameManager.Instance.GameOver();
        }
    }
}
