using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private PlayerScriptable _playerData;
    [SerializeField] private GameObject _propellersParent;
    [SerializeField] private Camera _pickerCamera;
        
    //Movement variables
    private Vector3 _mousePos;
    
    private float _lastMousePos;
    private float _swerve;
    private float _distanceToScreen;
    
    private bool _hasInput = false;
    private bool _initialized = false;
    private bool _hitLock = false;

    private GameObject _mainBody;
    private Rigidbody _playerRigidbody;
    private Transform _ballParent;



    #region Built-In Methods

    void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        SetPlayerState(false);

        _mainBody = transform.GetChild(0).gameObject; // CLOSE MAIN BODY BEFORE GAME START
        _mainBody.gameObject.SetActive(false);  
    }

    void Start()
    {
        GameManager.Instance.GameInitialized += OnPlayerInitialize;
    }

    void Update()
    {
        if (_initialized)
            GetInput();
        else
        {
            if (Input.GetMouseButtonDown(0) && !_initialized)
            {
                GameManager.Instance.OnGameInitialized();
            }
        }
    }

    void FixedUpdate()
    {
        if (_initialized)
        {
            if (GameManager.Instance.IsBallsFallInPool)
                return;


            FixPositionY();
            MovementSmooth();
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("BallTag"))
        {
            var ballParent = transform.GetChild(2);
            var ballRigidbody = other.transform.GetComponent<Rigidbody>();
            ballRigidbody.transform.SetParent(ballParent);

        }

        if (other.CompareTag("PropellerTag"))
        {
            Destroy(other.gameObject);
            SetPropellers(true);
        }

        if (other.CompareTag("StageBorderTag"))
        {
            Debug.Log("hit: " + other.gameObject.name);

            if (_hitLock) return;
            LevelManager.Instance.OnPlayerHitOnMovingPool();
            StartCoroutine(SetHitLock());
            GameManager.Instance.SetStageStatusToBallFallInsidePool();
            GameManager.Instance.SetBallsFallInPool();

            SetPropellers(false);
            PushBallsOnPool();
            StartCoroutine(RemoveBallsInPool());
            Destroy(other.gameObject, 3);
        }
    }
    


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BallTag"))
        {
            other.transform.SetParent(null);

        }

        if (other.CompareTag("SpawnStarterTag"))
        {
            if (!GameManager.Instance.IsNewLevelCreated)
            {
                GameManager.Instance.SetGameNewLevel();
                other.GetComponent<BoxCollider>().enabled = false;

                SetPlayerState(false);
            }
        }
    }

    public void SetPlayerState(bool activate)
    {
        if (activate)
        {
            _hasInput = true;
            _playerRigidbody.isKinematic = false;
        }
        else
        {
            _hasInput = false;
            _playerRigidbody.isKinematic = true;
        }
    }

    #endregion

    #region Movement


    private IEnumerator SetHitLock()
    {
        _hitLock = true;
        yield return new WaitForSeconds(2f);
        _hitLock = false;
    }
    private void FixPositionY()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform != null)
            {
                Vector3 position = new Vector3(transform.position.x, hit.transform.position.y, transform.position.z);
                _playerRigidbody.MovePosition(position);
            }
        }
    }
    private void GetInput()
    {
        Vector3 mo = Input.mousePosition;
                
        _distanceToScreen = _pickerCamera.WorldToScreenPoint(transform.position).z;
        _mousePos = _pickerCamera.ScreenToWorldPoint(new Vector3(mo.x, mo.y, _distanceToScreen));
        
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePos = _mousePos.x;
        }
        else if (Input.GetMouseButton(0))
        {
            _swerve = (_mousePos.x - _lastMousePos);
            _lastMousePos = _mousePos.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _swerve = 0;
        }
    }
    
    private void MovementSmooth()
    {
        if (!_hasInput) return;
        
        float swerveAmount = _playerData.HorizontalSpeed * _swerve;
        _playerRigidbody.velocity = new Vector3(swerveAmount, 0, 0) * Time.deltaTime;
        transform.Translate(0,0,_playerData.ForwardSpeed * Time.deltaTime);
        ClampPosition();


    }

    private void ClampPosition()
    {
        if (transform.position.x > _playerData.MaxBoundX)
        {
            transform.position = new Vector3(_playerData.MaxBoundX, transform.position.y, transform.position.z);
        }
        if (transform.position.x < _playerData.MinBoundX)
        {
            transform.position = new Vector3(_playerData.MinBoundX, transform.position.y, transform.position.z);
        }
    }

    #endregion

    #region Scale

    public void SetScale()
    {
        float nextScale = .025f;
        SetScaleValue(nextScale);
    }

    private void SetScaleValue(float scale)
    {
        transform.localScale += Vector3.one* scale;
    }
    
    public void ResetScale()
    {
        transform.DOScale(Vector3.one, 0f);
    }

    #endregion

    #region Controllers

    private void OnPlayerInitialize()
    {
        _ballParent = transform.GetChild(2);
        _mainBody.SetActive(true);
        SetPlayerState(true);
        _initialized = true;
        ResetScale();
    }
    
    private void PushBallsOnPool()
    
    {
        Rigidbody[] balls = _ballParent.GetComponentsInChildren<Rigidbody>(); 
        foreach (Rigidbody ball in balls)
        {
            ball.AddForce(Vector3.forward*_playerData.BallPush,ForceMode.Force);
        }
        
    }

    IEnumerator RemoveBallsInPool()
    {
        yield return new WaitForSeconds(1.5f);
        Rigidbody[] balls = _ballParent.GetComponentsInChildren<Rigidbody>(); 
        foreach (Rigidbody item in balls)
        {
            Destroy(item.gameObject);
        }
    }

    private void SetPropellers(bool status)
    {
        _propellersParent.SetActive(status);
    }

    #endregion
    
    

}

