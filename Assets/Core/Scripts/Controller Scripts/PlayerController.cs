using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerScriptable playerData;
    [SerializeField] private GameObject playerBallContainer;
    [SerializeField] private GameObject exitedBallsParent;
    [SerializeField] private GameObject _propellersParent;

    private bool _initialized = false;
    private float _lastMousePoint;
    private bool _isMouseDown = false;
    private Rigidbody _playerRigidbody;

    public bool HasInput = false;

    #region Built-In Methods


    void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.isKinematic = true;
        transform.GetChild(0).gameObject.SetActive(false); // CLOSE MAIN BODY BEFORE GAME START
    }

    void Start()
    {
        GameManager.Instance.GameInitialized += OnPlayerInitialize;

    }

    void Update()
    {
        if(_initialized)
            CheckInput();
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
            MovePlayer();

        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BallTag"))
        {
            other.gameObject.transform.parent = playerBallContainer.transform;
            LevelManager.Instance.FillBarCount();
        }
        
        if (other.CompareTag("PropellerTag"))
        {
            Destroy(other.gameObject);
            SetPropellers(true);
        }
        
        if (other.CompareTag("StageBorderTag"))
        {
            SetPropellers(false);
            Destroy(other.gameObject,3);
            GameManager.Instance.SetBallsFallInPool();
            PushBallsOnPool();
            GameManager.Instance.SetStageStatusToBallFallInsidePool();
            StartCoroutine(RemoveBallsInPool());
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BallTag"))
            other.gameObject.transform.parent = exitedBallsParent.transform;
        
        if (other.CompareTag("SpawnStarterTag"))
        {
            if (!GameManager.Instance.IsNewLevelCreated)
            {
                GameManager.Instance.SetGameNewLevel();
                other.GetComponent<BoxCollider>().enabled = false;
                
                HasInput = false;
                _playerRigidbody.isKinematic = false;

            }
        }
    }
    
    #endregion

    #region Movement
    
    private void FixPositionY()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform != null)
            {
                transform.DOMoveY(hit.transform.position.y, 0f);
            }
        }
    }

    private void CheckInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDown = true;
            _lastMousePoint = Input.mousePosition.x;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isMouseDown = false;
        }
    }
    
    private void MovePlayer()
    {
        if (!HasInput) return;
        
        if (_isMouseDown)
        {
            float difference = Input.mousePosition.x - _lastMousePoint;
        
            float xPos = transform.position.x + difference * Time.deltaTime * playerData.HorizontalSpeed;
            xPos = Mathf.Clamp(xPos, playerData.MinBoundX, playerData.MaxBoundX);
        
            Vector3 movementMouseDown = new Vector3(xPos, transform.position.y,
                transform.position.z + playerData.ForwardSpeed * Time.deltaTime);
            
            _playerRigidbody.MovePosition(movementMouseDown);
            _lastMousePoint = Input.mousePosition.x;
        }
        else
        {
            Vector3 movementMouseUp = new Vector3(transform.position.x, transform.position.y,
                transform.position.z + playerData.ForwardSpeed * Time.deltaTime);
            _playerRigidbody.MovePosition(movementMouseUp);
        }
    }


    
    #endregion

    #region Scale

    public void SetScale()
    {
        float nextScale = 1.1f;
        SetScaleValue(nextScale);
    }
    private void SetScaleValue(float scale)
    {
        playerData.PlayerScale = Vector3.one * scale;
    }

    private void ScalePlayer(float time)
    {
        transform.DOScale(playerData.PlayerScale, time);
    }

    public void ResetScale()
    {
        SetScaleValue(1f);
    }


    #endregion

    #region Controllers

    
    private void OnPlayerInitialize()
    {
        _playerRigidbody.isKinematic = false;
        transform.GetChild(0).gameObject.SetActive(true);
        HasInput = true;
        _initialized = true;
        ScalePlayer(0f); // get current Scale

    }
    private void PushBallsOnPool()
    {
        Rigidbody[] ballRigidbody = playerBallContainer.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in ballRigidbody)
        {
            rb.AddForce(transform.forward * playerData.BallPush * Time.deltaTime, ForceMode.Impulse);
        }
    }

    IEnumerator RemoveBallsInPool()
    {
        yield return new WaitForSeconds(1.5f);
        foreach (Transform item in exitedBallsParent.transform)
        {
            //item.gameObject.SetActive(false);
            Destroy(item.gameObject);
        }
    }

    private void SetPropellers(bool status)
    {
        _propellersParent.SetActive(status);
    }

    #endregion




}
