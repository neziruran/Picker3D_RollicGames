using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float ballScale = .1f;
    private Rigidbody _rigidbody;

    private bool _Active = false;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        transform.DOScale(ballScale, 0f);

        LevelManager.Instance.OnMovingPool += OnHitMovingPool;
    }

    private void OnHitMovingPool()
    {
        SetBallStatus(false);
    }

    private void SetBallStatus(bool status)
    {
        _Active = status;
    }

    void Update()
    {
        if (_Active)
        {
            _rigidbody.velocity.Normalize();
            _rigidbody.AddForce(Vector3.back*Time.deltaTime*15,ForceMode.VelocityChange); 
        }
            
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StageBorderTag"))
        {
            LevelManager.Instance.IncreaseCurrentBallCountInsidePool();
        }

        if (other.CompareTag("Player"))
        {
            SetBallStatus(true);
        }

    }
    
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            _Active = false;
        }
        
    }

}
