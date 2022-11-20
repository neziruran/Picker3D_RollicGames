using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class Flier : MonoBehaviour
{
    private const float SpawnThresholdZ = 40f;
    private const float EndThreshold = 60f;
    
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private float _spawnRate = .15f;
    private CinemachineDollyCart _dollyCart;
    private float _rotationSpeed = 5f;
    private Transform _rotatorPart;
    private Tween rotationTween;
    private bool _finished = false;

    

    private List<GameObject> _balls = new List<GameObject>();
    
    void Awake()
    {
        _rotatorPart = transform.GetChild(1);
        _dollyCart = GetComponent<CinemachineDollyCart>();
        _dollyCart.enabled = false;
    }

    private void Spawn()
    {
        GameObject ball = Instantiate(_ballPrefab, transform.GetChild(0).position, Quaternion.identity);
        _balls.Add(ball);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartRotate();
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hasTriggered = other.CompareTag("Player");

        if (hasTriggered)
        {
            _dollyCart.enabled = true;
            InvokeRepeating(nameof(Spawn),0f,_spawnRate);
        }
    }

    void Update()
    {
        if (_finished) return;
        
        if(_dollyCart.m_Position >= SpawnThresholdZ)
        {
            CancelInvoke();
        }
        if (_dollyCart.m_Position >= EndThreshold)
        {
            rotationTween.Kill();

            for (int i = 0; i < _balls.Count; i++)
            {
                var ball = _balls[i];
                //ball.SetActive(false);
                _balls.Remove(ball);
                Destroy(ball, 1f);
                if (i == _balls.Count)
                    _finished = true;
            }
            
        }

        
    }
    
    private void StartRotate()
    {
        Vector3 rot = new Vector3(0,360,0) * _rotationSpeed;
        rotationTween = _rotatorPart.DORotate(rot, 2f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
    }
}
