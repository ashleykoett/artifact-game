using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycast : MonoBehaviour
{
    [SerializeField] private string tagHandle = "Draggable";
    [SerializeField] private float snapDistance = 0.1f;
    [SerializeField] private InputAction interactAction;
    [SerializeField] private Plane workPlane;

    SherdGroup draggedGroup;
    private Vector3 dragOffset;
    private Sherd _selectedSherd;
    private Vector3 _selectedSnapPosition;
    private bool _selected;
    private Vector3 _offset; 
    private float _dragDepth = 10f; // prob wont use after the refactor
    private Plane dragPlane;
    
    private Vector3 _mousePos;
    private Vector3 _mouseScreenPos;
    private Vector3 _worldPosition;
    private bool _snapped;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
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
            _mouseScreenPos = new Vector3(_mousePos.x, _mousePos.y, _dragDepth);
            _worldPosition = Camera.main.ScreenToWorldPoint(_mouseScreenPos);
            
            if (_selectedSherd)
            {
                DragSherd();
            }
            else
            {
                FireRay();
            }
        }
        else
        {
            if (_selectedSherd)
            {
                // drop selected object
                _selectedSherd = null;
                _snapped = false;
            }
        }
    }

    private void DragSherd()
    {
        if (_snapped) return;

        Ray ray = Camera.main.ScreenPointToRay(_mousePos);

        if (dragPlane.Raycast(ray, out float dist))
            draggedGroup.transform.position = ray.GetPoint(dist) + dragOffset;

        while (draggedGroup.TrySnap())
            _snapped = true;
    }

    private void FireRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
    
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.GetComponent<Sherd>() != null)
        {
            _selectedSherd = hit.transform.GetComponent<Sherd>();
            draggedGroup = _selectedSherd.group;

            // plane at the group's position, facing the camera
            dragPlane = new Plane(-Camera.main.transform.forward, draggedGroup.transform.position);

            if (dragPlane.Raycast(ray, out float dist))
                dragOffset = draggedGroup.transform.position - ray.GetPoint(dist);
        }
    }
}
