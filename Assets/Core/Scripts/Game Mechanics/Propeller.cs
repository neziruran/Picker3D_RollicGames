using UnityEngine;
using DG.Tweening;

public class Propeller : MonoBehaviour
{
    [SerializeField] private bool _isRight = false;
    
    void Awake()
    {
        if(_isRight)
            transform.DORotate(new Vector3(0, -360, 0), 2f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        else
        {
            transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);

        }
    }
}
