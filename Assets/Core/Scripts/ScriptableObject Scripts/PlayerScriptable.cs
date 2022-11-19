﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player Data/PlayerData")]
public class PlayerScriptable : ScriptableObject
{
    [SerializeField] private float _horizontalSpeed = 0.4f;
    [SerializeField] private float _minBoundX, _maxBoundX;
    [SerializeField] private float _forwardSpeed = 2f;
    [SerializeField] private float _pushingPower = 5f;
    [SerializeField] private float _ballPush = 25f;
    [SerializeField] private Vector3 _playerScale = Vector3.one;
    public float HorizontalSpeed => _horizontalSpeed;
    public float MinBoundX => _minBoundX;
    public float MaxBoundX => _maxBoundX;
    public float ForwardSpeed => _forwardSpeed;
    
    public float BallPush => _ballPush;

    public Vector3 PlayerScale
    {
        get => _playerScale;
        set => _playerScale = value;
    }

    public float PushingPower
    {
        get =>_pushingPower;
        set => _pushingPower = value;
    } 
}
