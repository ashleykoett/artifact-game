using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycast : MonoBehaviour
{
    [SerializeField] private SherdPositionManager sherdPositionManager;
    [SerializeField] private string tagHandle = "Draggable";
    [SerializeField] private float snapDistance = 0.1f;
    [SerializeField] private InputAction interactAction;

    SherdGroup draggedGroup;
    private Vector3 dragOffset;
    private Sherd _selectedSherd;
    private Vector3 _selectedSnapPosition;
    private bool _selected;
    private Vector3 _offset; 
    private float _dragDepth = 10f;
    
    private Vector3 _mousePos;
    private Vector3 _mouseScreenPos;
    private Vector3 _worldPosition;

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
            }
        }
    }

    private void DragSherd()
    {
        /*
        Vector3 sherdPos = new Vector3(_worldPosition.x + _offset.x, _worldPosition.y + _offset.y, _selectedSherd.transform.position.z);

        // see if we're close to the snap point

        if (Vector3.Distance(sherdPos, _selectedSnapPosition) < snapDistance)
        {
            _selectedSherd.transform.position = _selectedSnapPosition;
        }
        else
        {
            _selectedSherd.transform.position = sherdPos;
        }


        _selectedSherd.transform.position = sherdPos;
        */
        draggedGroup.transform.position = _worldPosition + dragOffset;
        while (draggedGroup.TrySnap()) { } // keep checking until no more merges
        
    }

    private void FireRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit, 100f) && hit.transform.gameObject.GetComponent<Sherd>() != null)
        {
            _selectedSherd = hit.transform.gameObject.GetComponent<Sherd>();
            draggedGroup = _selectedSherd.group;
            dragOffset = draggedGroup.transform.position - _worldPosition;
                    
            // calculate mouse offset
            // _offset  = _selectedSherd.transform.position - _worldPosition;
            // _selectedSnapPosition = sherdPositionManager.GetSherdPosition(_selectedSherd);
        }
    }
}
