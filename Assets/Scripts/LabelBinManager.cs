using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;

public class LabelBinManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _labelBinParent;
    [SerializeField] private float _animationDuration = 1f;

    private bool _open = true;
    private float _closedY;
    private float _openY;

    private void Start()
    {
        _closedY = -_labelBinParent.rect.height;
        _openY = 0f;
        _labelBinParent.anchoredPosition = new Vector2(_labelBinParent.anchoredPosition.x, _closedY);
        _open = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_open)
        {
            Tween.UIAnchoredPositionY(_labelBinParent, _closedY, _animationDuration, Ease.OutBack);
            _open = false;
        }
        else
        {
            Tween.UIAnchoredPositionY(_labelBinParent, _openY, _animationDuration, Ease.InBack);
            _open = true;
        }
    }
}
