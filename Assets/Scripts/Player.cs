using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    //private float moveSpeed = 5f / Constants.TICKS_PER_SEC;

    private object[] inputs;

    public Rigidbody _rb;

    private float _rotationDirectionAndSpeed = 0;
    private float _rotTargetSpeed;
    [Header("Rotation")]
    [SerializeField] private float _maxRotSpeed = 1.5f;
    [SerializeField] private float _rotationAdded = 0.3f;
    [SerializeField] private float _letGoMultiplier = 75;

    [Header("UI")]
    //[SerializeField] private RectTransform _wheel;
    //[SerializeField] private RectTransform _wheelParent;

    [SerializeField] private float _forwardForce = 1.5f;
    [SerializeField] private float _rotationForwardMultiplier;
    /*[Range(0,1)]
    [SerializeField] private float _onRotationVelocityLoss;*/

    private Vector3 _mousePos;


    private bool _isWheelHeld;
    private int _wheelSlice;
    private List<int> _wheelMemory;

    public void Initialize(int _id, string _username)
    {
        _wheelMemory = new List<int>();
        id = _id;
        username = _username;
        _rb = GetComponent<Rigidbody>();
        _maxRotSpeed = 1.5f;
        _rotationAdded = 0.3f;
        _letGoMultiplier = 75;
        _forwardForce = 1.5f;

        inputs = new object[4];
    }
    

    public void Update()
    {
        if (inputs[0] != null && (bool)inputs[0])
        {
            Debug.Log("forward");
            GoForward();
            
        }
        if (inputs[1] != null && inputs[1] is float && (float)inputs[1] != 0)
        {
            Debug.Log("_rotTargetSpeed: " + inputs[1]);
            Debug.Log("_rotationDirectionAndSpeed: " + inputs[2]);
            RotateShip((float)inputs[2]);
        }
        
        
        /*
        if (Input.GetMouseButtonDown(0))
        {
            _isWheelHeld = true;
            HoldWheelOffset();
        }

        if (Input.GetMouseButton(0))
        {
            RotateWheel();
            GetRotationDirectionAndTargetSpeed(
                GetWheelSlice(_wheel.rotation.eulerAngles.z));
            _rotationDirectionAndSpeed =
                Mathf.Lerp(_rotationDirectionAndSpeed, _rotTargetSpeed, Time.deltaTime);
            RotateShip(_rotationDirectionAndSpeed);

        }

        if (Input.GetMouseButtonUp(0))
        {
            _isWheelHeld = false;
        }

        if (!_isWheelHeld && Mathf.Abs(_rotTargetSpeed) > 0.05f)
        {
            StopRotating();
            WheelLetGo(_rotationDirectionAndSpeed * _letGoMultiplier);
        }*/

    }
    /*
    private void HoldWheelOffset()
    {
        _wheel.SetParent(_wheelParent.parent);
        RotateWheel();
        _wheel.SetParent(_wheelParent);
    }
    */
    private void WheelLetGo(float f)
    {
        //_wheelParent.rotation = Quaternion.Euler(0, 0, _wheelParent.rotation.z + f);
    }
    /*
    private void RotateWheel()
    {
        _mousePos = Input.mousePosition;
        Vector3 diffrence = _mousePos - _wheel.position;
        float rotationZ = Mathf.Atan2(diffrence.y, diffrence.x) * Mathf.Rad2Deg;
        _wheelParent.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
    */

    private void RotateShip(float force)
    {
        _rb.angularVelocity = new Vector3(0, force, 0);
        Debug.Log("rotate");
        //Vector3 targetVelocity = (_rb.velocity * 0.5f) + (transform.forward * _rb.velocity.magnitude * 0.5f);
        //_rb.velocity = Vector3.Lerp(_rb.velocity, targetVelocity, Time.deltaTime * _rotationForwardMultiplier);
        //Debug.Log("rotate " + transform.rotation);
        //Debug.Log(_rotationForwardMultiplier + " 1");
        //Debug.Log(_rb.velocity.magnitude + " 2");
        //Debug.Log(_rb.velocity + " 3");
        //this.transform.rotation = (Quaternion)inputs[2];
        ServerSend.PlayerRotation(this); // tell everybody that i rotate
    }

    private int GetWheelSlice(float angle)
    {
        if (angle >= 0f && angle < 45f) //Slice 0
        {
            _wheelSlice = 0;
        }

        if (angle >= 45f && angle < 90f) //Slice 1
        {
            _wheelSlice = 1;
        }

        if (angle >= 90 && angle < 135f) //Slice 2
        {
            _wheelSlice = 2;
        }

        if (angle >= 135f && angle < 180f) //Slice 3
        {
            _wheelSlice = 3;
        }

        if (angle >= 180f && angle < 225f
            || angle >= -180f && angle < -135f) // Slice 4
        {
            _wheelSlice = 4;
        }

        if (angle >= 225 && angle < 270f
            || angle >= -135f && angle < -90f)//Slice 5
        {
            _wheelSlice = 5;
        }

        if (angle >= 270 && angle < 315f
            || angle >= -90f && angle < -45f) //Slice 6
        {
            _wheelSlice = 6;
        }

        if (angle >= 315 && angle < 360f
            || angle >= -45f && angle < 0f) //Slice 7
        {
            _wheelSlice = 7;
        }

        return _wheelSlice;
    }

    private void GetRotationDirectionAndTargetSpeed(int i)
    {
        int _currentWheelSlice;
        if (_wheelMemory.Count != 0)
        {
            _currentWheelSlice = _wheelMemory.Last();
        }
        else
        {
            _currentWheelSlice = i;
            _wheelMemory.Add(i);
        }


        if (i != _currentWheelSlice)
        {
            _wheelMemory.Add(i);
        }
        else
        {
            StopRotating();
        }


        if (_currentWheelSlice < _wheelMemory.Last())  //Counter clockwise
        {
            _rotTargetSpeed -= _rotationAdded;

            if (_wheelMemory.Last() == 7 && _currentWheelSlice == 0) // clockwise, exception
            {
                _rotTargetSpeed += _rotationAdded;
            }
        }
        else if (_currentWheelSlice > _wheelMemory.Last()) //clockwise
        {
            _rotTargetSpeed += _rotationAdded;
            if (_currentWheelSlice == 7 && _wheelMemory.Last() == 0) // Clockwise, exception
            {
                _rotTargetSpeed -= _rotationAdded;
            }
        }
        if (Mathf.Abs(_rotTargetSpeed) > _maxRotSpeed)
        {
            if (_rotTargetSpeed > 0)
            {
                _rotTargetSpeed = _maxRotSpeed;
            }
            else
            {
                _rotTargetSpeed = -_maxRotSpeed;
            }
        }



    }

    private void StopRotating()
    {
        _rotTargetSpeed = Mathf.Lerp(_rotTargetSpeed, 0, Time.deltaTime);
        _rotationDirectionAndSpeed = Mathf.Lerp(_rotTargetSpeed, 0, Time.deltaTime);
    }

    public void GoForward()
    {
        _forwardForce = 1.5f;
        Debug.Log("forward");
        _rb.AddForce(transform.forward * _forwardForce);
        //this.transform.position = (Vector3)inputs[2];
        ServerSend.PlayerPosition(this); // tell everybody that i move
        //ServerSend.PlayerRotation(this);
    }
    

    public void SetInput(object[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

}
