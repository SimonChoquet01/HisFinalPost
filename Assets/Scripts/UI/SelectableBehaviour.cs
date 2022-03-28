using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //Where the interfaces come from
using UnityEngine.InputSystem.LowLevel;

//Trick learned in class to resolve Mouse and Controller conflict for UI
public class SelectableBehaviour : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, IPointerDownHandler //These interfaces allow you to implement the callback functions used below
{

    public void OnDeselect(BaseEventData eventData)
    {
        //Retrieve any selectable component (A.k.a components that derive from Selectable)
        GetComponent<Selectable>().OnPointerExit(null); //Make sure we deselect *and* un-highlight (Very important to stop player from using controller to select something and mouse for something else)
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Only relevant for buttons which is why we look for only a button. (Example: If this is preformed on a slider it will return null and not execute the following part)
        if (eventData.selectedObject.GetComponent<Button>() != null)
        {
            GetComponent<Button>().onClick.Invoke(); //Trigger immediately instead of waiting for release
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Retrieve any selectable component (A.k.a components that derive from Selectable)
        GetComponent<Selectable>().Select(); //Force select whatever is highlighted (Same reason as the OnDeselect)
    }
}
