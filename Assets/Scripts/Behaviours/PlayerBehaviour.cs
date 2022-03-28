//using System.Collections;
//using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    //--- Editor Parameters ---
    [Header("Movement settings")]
    [SerializeField] private float _playerGravity = 9.81f; //This technically can't just be changed in real life but it feels weird in videogames.
    [SerializeField] private float _walkSpeed = 6.0f;
    [SerializeField] private float _runSpeed = 8.0f;
    [SerializeField] private float _jumpForce = 4.0f;
    [Header("Sounds")]
    [SerializeField] private AudioSource _leftSource;
    [SerializeField] private AudioSource _rightSource;
    [SerializeField] private AudioClip _swingSound;
    [SerializeField] private AudioClip _hitSound;
    [Header("UI")]
    [SerializeField] private GameObject _blinder;
    [SerializeField] private Text _healthInfo;
    [Header("Other")]
    [SerializeField] private GameObject _camera;

    //--- Public Variables ---
    [HideInInspector] public float sensitivity = 0.3f;

    //--- Private Variables ---
    private CharacterController _controller;
    private Animator _animator;

    private Vector2 _moveStick = new Vector2();
    private Vector2 _lookStick = new Vector2();
    private bool _sprinting = false;
    private bool _jump = false;
    private bool _rightFiring = false;
    private bool _leftFiring = false;

    private Vector3 _characterVelocity = new Vector3();
    private float _viewportVertical = 0.0f; //The x angle stored (to fix clamping issue I've been having)
    private int _health = 100;
    private float _lastHit = 2.0f;

    //--- Input System Events ---
    public void OnRightFire(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Ended || GameManager.Instance.Paused) return;
        if (context.performed == true)
        {
            if (!_rightFiring)
            {
                _rightFiring = true;
                _animator.SetTrigger("R_Fire");
            }
        }
    }

    public void OnLeftFire(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Ended || GameManager.Instance.Paused) return;
        if (context.performed == true)
        {
            if (!_leftFiring)
            {
                _leftFiring = true;
                _animator.SetTrigger("L_Fire");
            }
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveStick = context.ReadValue<Vector2>();
        if (_moveStick.magnitude > 1)
        {
            _moveStick.Normalize();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        _lookStick = context.ReadValue<Vector2>();
        _lookStick *= sensitivity;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.Ended) return;
        _sprinting = context.performed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _jump = context.performed;
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.TogglePause();
        }
    }
    
    //--- Public Methods ---
    public void Blind()
    {
        _blinder.SetActive(true);
    }

    public void Unblind()
    {
        _blinder.SetActive(false);
    }
    public void Hurt(int damage)
    {
        if (_health > 0)
        {
            _health -= damage;
            _lastHit = 0.0f;
        }
        else
        {
            _health = 0;
        }
    }
    public static void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //--- Animation Events ---
    public void R_Swing()
    {
        _rightSource.PlayOneShot(_swingSound);
    }
    public void L_Swing()
    {
        _leftSource.PlayOneShot(_swingSound);
    }
    public void R_AttackRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 2))
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.gameObject.GetComponent<Monster>().Hurt(1f);
            }
            else
            {
                _rightSource.PlayOneShot(_hitSound);
            }
        }
    }
    public void L_AttackRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 2))
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.gameObject.GetComponent<Monster>().Hurt(0.5f);
            }
            else
            {
                _leftSource.PlayOneShot(_hitSound);
            }
        }
    }

    public void R_Done()
    {
        _rightFiring = false;
    }
    public void L_Done()
    {
        _leftFiring = false;
    }

    
    //--- Unity Methods ---
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        LockMouse();
        sensitivity = PlayerPrefs.GetFloat("Sens", 3.5f) * 0.1f;
    }

    void Update()
    {
        _healthInfo.text = "Health: " + _health; //Update GUI
        if (!GameManager.Instance.Ended && !GameManager.Instance.Paused)
        {
            //Handle camera shake
            if (_lastHit < 0.5f)
            {
                _lastHit += Time.deltaTime;
                _camera.transform.localPosition = new Vector3(0, 1.7f, 0) + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));
                _camera.transform.localPosition = new Vector3(0, 1.7f, 0) + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));
            }
            else
            {
                _camera.transform.localPosition = new Vector3(0, 1.7f, 0);
            }

            //Handle game over
            if (_health <= 0 && !GameManager.Instance.Ended)
            {
                GameManager.Instance.GameOver();
            }

            //Handle look and clamping
            transform.Rotate(new Vector3(0, 1, 0), _lookStick.x);
            _viewportVertical -= _lookStick.y; //Invert
            _viewportVertical = Mathf.Clamp(_viewportVertical, -90, 90);
            _camera.transform.localEulerAngles = new Vector3(_viewportVertical, 0.0f, 0.0f);

            if (_controller.isGrounded)
            {
                //Handle move
                _characterVelocity = new Vector3(); //Clear velocity
                _characterVelocity += transform.right * _moveStick.x; //Move sideways by the amount we are moving the stick;
                _characterVelocity += transform.forward * _moveStick.y; //Move back and forth by the amount we are moving the stick;
                _characterVelocity *= (_sprinting ? _runSpeed : _walkSpeed); //Make it faster if we're sprinting

                //Handle anim
                if (_characterVelocity.magnitude > 0)
                {
                    _animator.SetBool("Walking", true);
                    if (_sprinting)
                    {
                        _animator.SetBool("Sprinting", true);
                    }
                    else
                    {
                        _animator.SetBool("Sprinting", false);
                    }
                }
                else
                {
                    _animator.SetBool("Sprinting", false);
                    _animator.SetBool("Walking", false);
                }


                //Handle jump
                if (_jump)
                {
                    _jump = false;
                    _characterVelocity.y = _jumpForce;
                }
            }

            _characterVelocity.y -= _playerGravity * Time.deltaTime; //Gravity is typically applied in Meters/Second^2 or Meters per Second per Second (This is why I multiply the Y twice or else falling is constant and unrealistic)
            _controller.Move(_characterVelocity * Time.deltaTime);
        }
        else
        {
            _animator.SetBool("Sprinting", false);
            _animator.SetBool("Walking", false);
        }
    }
}
