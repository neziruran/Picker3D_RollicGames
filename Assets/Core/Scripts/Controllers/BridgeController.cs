using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    private Transform _leftPiece, _rightPiece;

    private bool _executed = false;
    [SerializeField] private Color bridgeColor;
    
    void Awake()
    {
        _rightPiece = transform.GetChild(0);
        _leftPiece = transform.GetChild(1);
        
    }

    void Start()
    {
        _leftPiece.GetComponentInChildren<Renderer>().material.color = bridgeColor;
        _leftPiece.GetComponentInChildren<Renderer>().material.color = bridgeColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hasTriggered = other.CompareTag("Player");

        if (hasTriggered && !_executed)
        {
            GameManager.Instance.CurrentBridge = this;
            _executed = true;
        }
    }

    public void OpenBridge()
    {
        StartCoroutine(OpenBridgeInSeconds(0f));

    }

    private IEnumerator OpenBridgeInSeconds(float t)
    {
        yield return new WaitForSeconds(t);
        Open();
        
    }
    private void Open()
    {
        Quaternion leftRot = Quaternion.Euler(0,0,-60);
        _leftPiece.DOLocalRotateQuaternion(leftRot, 1f);
        
        Quaternion rightRot = Quaternion.Euler(0,180,-60);
        _rightPiece.DOLocalRotateQuaternion(rightRot, 1f);
    }
    
}
