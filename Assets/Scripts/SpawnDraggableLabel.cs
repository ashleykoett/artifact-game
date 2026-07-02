using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnDraggableLabel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private DraggableLabel _spawnedLabel;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject go = Instantiate(gameObject, transform.root);
        go.transform.position = transform.position;
        go.GetComponent<SpawnDraggableLabel>().enabled = false;
        _spawnedLabel = go.GetComponent<DraggableLabel>();
        _spawnedLabel.enabled = true;
        _spawnedLabel.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) => _spawnedLabel?.OnDrag(eventData);

    public void OnEndDrag(PointerEventData eventData)
    {
        _spawnedLabel?.OnEndDrag(eventData);
        _spawnedLabel = null;
    }
}
