using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class DraggableLabel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static event Action OnUIDragStart;
    public static event Action OnUIDragEnd;
    
    RectTransform rt;

    private Vector2 _originalPosition;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        _originalPosition = rt.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.position = eventData.position;
        OnUIDragStart?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Tag"))
        {
            RectTransform tagCanvasRT = hit.transform.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tagCanvasRT, eventData.position, Camera.main, out localPoint);

            GameObject label = Instantiate(gameObject, tagCanvasRT);
            label.GetComponent<RectTransform>().anchoredPosition = localPoint;
            label.GetComponent<DraggableLabel>().enabled = false;
        }
        
        OnUIDragEnd?.Invoke();
        rt.position = _originalPosition;
    }
}
