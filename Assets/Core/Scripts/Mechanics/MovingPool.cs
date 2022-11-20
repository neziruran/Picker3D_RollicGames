using UnityEngine;
using DG.Tweening;

public class MovingPool : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DOMoveY(.25f, .5f, false).SetEase(Ease.OutBack);

        transform.DOScaleZ(.16f, 0f);
    }
}
