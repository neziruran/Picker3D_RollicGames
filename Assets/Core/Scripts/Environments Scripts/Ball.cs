﻿using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float ballScale = .1f;
    private Rigidbody _rigidbody;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        transform.DOScale(ballScale, 0f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StageBorderTag"))
            LevelManager.Instance.IncreaseCurrentBallCountInsidePool();
    }
}