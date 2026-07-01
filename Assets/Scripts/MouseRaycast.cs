using UnityEngine;
using UnityEngine.InputSystem;


// Make this generic
public class MouseRaycast : MonoBehaviour
{
    [SerializeField] private string tagHandle = "Draggable";
    [SerializeField] private float snapDistance = 0.1f;
    [SerializeField] private InputAction interactAction;
    [SerializeField] private BoxCollider dragBox;

    SherdGroup draggedGroup;
    private Vector3 dragOffset;
    private GameObject _selectedObject;
    // private Sherd _selectedSherd;
    private Vector3 _selectedSnapPosition;
    private bool _selected;
    private Vector3 _offset; 
    private float _dragDepth = 0f; // prob wont use after the refactor
    private Plane _dragPlane;
    private Bounds _dragBoxBounds;
    private Vector3 _boundsOffset;
    private Vector3 _boundsExtents;
    
    private Vector3 _mousePos;
    private bool _snapped;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        _dragBoxBounds = dragBox.bounds;
    }
    
    // Mouse shoots ray looking for the tag set in the inspector
    // If the tag is found, the piece moves while the mouse is held down
    // Z position always stays the same
    void Update()
    {
        bool interacting = interactAction.IsPressed();
        
        if (interacting) 
        {
            _mousePos = Mouse.current.position.ReadValue();
            
            if (_selectedObject)
            {
                SelectedObjectBehavior();
            }
            else
            {
                FireRay();
            }
        }
        else
        {
            if (_selectedObject)
            {
                // drop selected object
                _selectedObject = null;
                _snapped = false;
            }
        }
    }

    private void FireRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.CompareTag("Draggable"))
        {
            if (hit.transform.GetComponent<Sherd>() != null)
            {
                OnSelectSherd(ray, hit);
                return;
            }

            _selectedObject = hit.transform.gameObject;
            _dragPlane = new Plane(-Camera.main.transform.forward, hit.transform.position);

            if (_dragPlane.Raycast(ray, out float dist))
                dragOffset = hit.transform.position - ray.GetPoint(dist);

            Bounds cb = _selectedObject.GetComponent<Collider>().bounds;
            _boundsOffset = cb.center - _selectedObject.transform.position;
            _boundsExtents = cb.extents;
        }
    }

    private void OnSelectSherd(Ray ray, RaycastHit hit)
    {
        // _selectedSherd = hit.transform.GetComponent<Sherd>();
        _selectedObject = hit.transform.gameObject;
        draggedGroup = _selectedObject.GetComponent<Sherd>().group;

        // plane at the group's position, facing the camera
        _dragPlane = new Plane(-Camera.main.transform.forward, draggedGroup.transform.position);

        if (_dragPlane.Raycast(ray, out float dist))
            dragOffset = draggedGroup.transform.position - ray.GetPoint(dist);

        Bounds b = draggedGroup.GetGroupBounds();
        _boundsOffset = b.center - draggedGroup.transform.position;
        _boundsExtents = b.extents;
    }

    private void SelectedObjectBehavior()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        Vector3 pos = new Vector3();
        
        // Drag Sherd
        if (_selectedObject.GetComponent<Sherd>() != null)
        {
            if (_snapped) return;
    
            if (_dragPlane.Raycast(ray, out float dist))
                pos = ray.GetPoint(dist) + dragOffset;
            
            draggedGroup.transform.position = ClampWithCachedBounds(pos, _boundsOffset, _boundsExtents);

            while (draggedGroup.TrySnap())
                _snapped = true;
        }
        // Drag other object
        else
        {
            if (_dragPlane.Raycast(ray, out float dist))
                pos = ray.GetPoint(dist) + dragOffset;
            
            _selectedObject.transform.position = ClampWithCachedBounds(pos, _boundsOffset, _boundsExtents);
        }
    }

    private Vector3 ClampWithCachedBounds(Vector3 pos, Vector3 boundsOffset, Vector3 extents)
    {
        if (pos.x + boundsOffset.x - extents.x < _dragBoxBounds.min.x) pos.x = _dragBoxBounds.min.x - boundsOffset.x + extents.x;
        if (pos.x + boundsOffset.x + extents.x > _dragBoxBounds.max.x) pos.x = _dragBoxBounds.max.x - boundsOffset.x - extents.x;
        if (pos.y + boundsOffset.y - extents.y < _dragBoxBounds.min.y) pos.y = _dragBoxBounds.min.y - boundsOffset.y + extents.y;
        if (pos.y + boundsOffset.y + extents.y > _dragBoxBounds.max.y) pos.y = _dragBoxBounds.max.y - boundsOffset.y - extents.y;
        return pos;
    }
}
