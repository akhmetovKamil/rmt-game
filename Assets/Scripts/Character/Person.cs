using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Character;
using Assets.Scripts;

public class Person : Character
{
    [SerializeField]
    private float runSpeed = 2.3f;

    [SerializeField]
    private float walkSpeed = 1f;

    [SerializeField]
    private float movementTransitionSpeed = 8f;

    [SerializeField]
    private float _gravity = 11f;

    [SerializeField]
    private float smoothTime = 0.03f;

    private float _currentSpeed = 0.0f;


    private CharacterController _characterController;
    private Animator _animator;
    private Vector3 _moveDirection;
    private Vector3 _velocity;
    private float _currentVelocity;
    private bool _isSitting = false;
    private List<Weapon> _weaponList = new();
    private Weapon _currentWeapon;
    private MagicFire _magicFire;
    private MeeleSword _meeleSword;
    private MeeleAxe _meeleAxe;
    private MagicWind _magicWind;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _animator = GetComponentInChildren<Animator>();
        _magicFire = GetComponent<MagicFire>();
        _meeleSword = GetComponent<MeeleSword>();
        _meeleAxe = GetComponent<MeeleAxe>();
        _magicWind = GetComponent<MagicWind>();
    }

    void Update()
    {
        InputDirection();
    }

    void FixedUpdate()
    {
        Move();
    }
    void ChangeWeapon(WeaponTypes weaponType)
    {

    }
    protected private void Move()
    {
        if (!_isSitting)
        {
            Movement(_moveDirection);
            Rotation(_moveDirection);
            GravityMovement(_characterController.isGrounded);
        }
        if (_moveDirection == Vector3.zero)
            StartCoroutine(Sitting());
    }
    void InputDirection()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        _moveDirection = new Vector3(x, 0.0f, z);
    }
    void Movement(Vector3 direction)
    {
        //_currentSpeed = Mathf.SmoothStep(_currentSpeed, walkSpeed, 2 * Time.deltaTime);
        if (direction != Vector3.zero && _animator.GetBool("IsWalking") && Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = Mathf.SmoothStep(_currentSpeed, runSpeed, movementTransitionSpeed * Time.deltaTime);
            _animator.SetBool("IsRunning", true);
        }
        else if (direction != Vector3.zero)
        {
            _currentSpeed = Mathf.SmoothStep(_currentSpeed, walkSpeed, movementTransitionSpeed * Time.deltaTime);
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _currentSpeed = Mathf.SmoothStep(_currentSpeed, 0, movementTransitionSpeed * Time.deltaTime);
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsWalking", false);
        }


        _characterController.Move(direction * _currentSpeed * Time.deltaTime);
    }

    IEnumerator Sitting()
    {
        bool isAnimSitting = _animator.GetBool("IsSitting");
        if (Input.GetKey(KeyCode.C) && !isAnimSitting)
        {
            _isSitting = true;
            _animator.SetBool("IsSitting", true);
        }
        else if (Input.anyKey && !Input.GetKey(KeyCode.C) && _isSitting && isAnimSitting)
        {
            _animator.SetBool("IsSitting", false);
            _currentSpeed = 0;
            yield return new WaitForSeconds(0.6f);
            _isSitting = false;
        }
    }

    void GravityMovement(bool isGrounded)
    {
        if (isGrounded && _velocity.y < 0.0f)
            _velocity.y = -1f;
        else
        {
            _velocity.y -= _gravity * Time.fixedDeltaTime;
            _characterController.Move(_velocity * Time.fixedDeltaTime);
        }
    }

    void Rotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
    public UpgradableParametr Upgrade(string weapon, string param)
    {
        UpgradableParametr result = new();
        switch (weapon)
        {
            case "sword":
                {
                    result = _meeleSword.Upgrade(param);
                    break;
                }
            case "axe":
                {
                    result = _meeleAxe.Upgrade(param);
                    break;
                }
            case "fire":
                {
                    result = _magicFire.Upgrade(param);
                    break;
                }
            case "wind":
                {
                    result = _magicWind.Upgrade(param);
                    break;
                }
        }
        return result;
    }
}
