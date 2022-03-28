//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;

public class MonolithBehaviour : MonoBehaviour
{
    //--- Private Variables ---
    private Collider _collider;
    private bool _isBlinding = false;

    //--- Unity Event ---
    void Start()
    {
        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            _collider = GetComponentInChildren<Collider>();
        }
    }

    void Update()
    {
        //Blind the player if they are stuck inside the monolith (To avoid seeing through the clear side of the polygons)
        if ( Vector3.Distance(GameManager.Instance.Player.transform.position+ (GameManager.Instance.Player.transform.up*2), transform.position) < 2)
        {
            if (!_isBlinding)
            {
                GameManager.Instance.Player.GetComponent<PlayerBehaviour>().Blind();
                _isBlinding = true;
            }
        }
        else
        {
            if (_isBlinding)
            {
                GameManager.Instance.Player.GetComponent<PlayerBehaviour>().Unblind();
                _isBlinding = false;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerBehaviour>().Hurt(20);
        }
    }

    //--- Animation Events---
    public void StartMoving()
    {
        _collider.isTrigger = true;
    }

    public void StopMoving()
    {
        _collider.isTrigger = false;
    }

}
