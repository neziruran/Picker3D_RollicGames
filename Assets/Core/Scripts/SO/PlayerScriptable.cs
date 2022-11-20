using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player Data/PlayerData")]
public class PlayerScriptable : ScriptableObject
{
    [SerializeField] private float _horizontalSpeed = 0.4f;
    [SerializeField] private float _minBoundX, _maxBoundX;
    [SerializeField] private float _forwardSpeed = 2f;
    [SerializeField] private float _ballPush = 25f;
    public float HorizontalSpeed => _horizontalSpeed;
    public float MinBoundX => _minBoundX;
    public float MaxBoundX => _maxBoundX;
    public float ForwardSpeed => _forwardSpeed;
    
    public float BallPush => _ballPush;
    
    
}
