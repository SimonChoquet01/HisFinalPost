//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    //--- Editor Parameters---

    //speeds
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3.5f;
    [SerializeField] private float _runSpeed = 7.0f;
    [Header("Sounds")]
    [SerializeField] private AudioClip _gallopSound;
    [SerializeField] private AudioClip[] _howlSound;
    [SerializeField] private AudioClip _fadeSound;
    [SerializeField] private AudioClip _hitSound;
    [Header("Other")]
    [SerializeField] private LayerMask _playerOnly; //This is for when the monster attacks we want to only return a player
    [SerializeField] private Renderer _renderer; //This will later allow me to destroy the monster when it's not being looked at

    //--- Private Variables ---

    private NavMeshAgent _navAgent;
    private Animator _animator;
    private AudioSource _audioSource;
    private Collider _collider;

    private float _health = 3;
    private bool _canMove = true;
    private bool _dead = false;

    //--- Public Variables ---
    [HideInInspector] public GameObject Target;

    //--- Animator Events ---
    public void Gallop()
    {
        _audioSource.PlayOneShot(_gallopSound);
    }
    public void AttackCheck()
    {
        Collider[] colliders = Physics.OverlapCapsule(this.transform.position + (this.transform.forward * 1.3f), this.transform.position + (this.transform.forward * 1.3f) + (this.transform.up * 2.0f), 1f, _playerOnly);
        foreach(Collider col in colliders)
        {
            col.GetComponent<PlayerBehaviour>().Hurt(10);
        }
    }

    public void CanMove()
    {
        _canMove = true;
    }

    //--- Public Methods ---
    public void Clean()
    {
        Destroy(this.gameObject);
    }
    public void Howl()
    {
        CancelInvoke();
        Invoke("Howl", Random.Range(2.0f, 8.0f));
        _audioSource.PlayOneShot(_howlSound[Random.Range(0, _howlSound.Length)]);
    }

    public void Kill()
    {
        _canMove = false;
        _animator.SetTrigger("Death");
        CancelInvoke();
        _dead = true;
        if (!GameManager.Instance.Ended) GameManager.Instance.AddRandomMonsters(1);
    }
    public void Hurt(float damage)
    {
        if (this._health > 0)
        {
            _health -= damage;
            _navAgent.isStopped = true;
            _canMove = false;
            if (this._health > 0)
            {
                _animator.SetTrigger("Hit" + Random.Range(1,3));
                Howl();
            }
            else
            {
                Kill();
            }
        }
        _audioSource.PlayOneShot(_hitSound);
    }


    //--- Unity Methods ---
    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        Invoke("Howl", Random.Range(2.0f, 8.0f));
    }
    void FixedUpdate()
    {
        if (!_dead)
        {
            //Handles run if far
            if ((Target.transform.position - transform.position).magnitude > 10)
            {
                _navAgent.speed = _runSpeed;
            }
            else
            {
                _navAgent.speed = _walkSpeed;
            }

            //Stop and attack + anims
            if (((Target.transform.position - transform.position).magnitude < 2 && _canMove) && Vector3.Angle(transform.forward, Target.transform.position - transform.position) < 20) 
            {
                _navAgent.isStopped = true;
                _canMove = false;
                _animator.SetTrigger("Attack");
                Howl();
            }
            else if (_canMove)
            {
                _navAgent.isStopped = false;
                _navAgent.SetDestination(Target.transform.position);
                _animator.SetBool("Walk", true);
                if (_navAgent.speed >= _runSpeed)
                {
                    _animator.SetBool("Run", true);
                }
                else
                {
                    _animator.SetBool("Run", false);
                }
            }
            if (_navAgent.isStopped)
            {
                _animator.SetBool("Walk", false);
                _animator.SetBool("Run", false);
            }
            if (GameManager.Instance.Ended)
            {
                _navAgent.isStopped = true;
                _canMove = false;
                Kill();
            }
        }
        else
        {
            //Dead body disappears when not looked at.
            if (!_renderer.isVisible)
            {
                if (!IsInvoking("Clean"))
                {
                    _audioSource.PlayOneShot(_fadeSound);
                    _renderer.enabled = false;
                    Invoke("Clean", 1.0f); //Just to make sure the sound plays first.
                }
            }
        }
    }
}
