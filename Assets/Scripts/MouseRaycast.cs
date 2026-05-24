using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycast : MonoBehaviour
{
    [SerializeField] private string tagHandle = "Draggable";
    [SerializeField] private InputAction interactAction;

    private GameObject _selectedObject;
    private bool _selected;
    private Vector3 _offset; 
    private float _dragDepth = 10f;

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
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseScreenPos = new Vector3(mousePos.x, mousePos.y, _dragDepth);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            
            if (_selectedObject)
            {
                // move selected object
                _selectedObject.transform.position = new Vector3(worldPosition.x + _offset.x, worldPosition.y + _offset.y, _selectedObject.transform.position.z);
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;
            
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.transform.CompareTag(tagHandle))
                    {
                        
                        Debug.Log("Hit: " + hit.transform.gameObject.tag);
                        _selectedObject = hit.transform.gameObject;
                        
                        // calculate mouse offset
                        _offset  = _selectedObject.transform.position - worldPosition;
                    }
                }
            }
        }
        else
        {
            if (_selectedObject)
            {
                // drop selected object
                _selectedObject = null;
            }
        }
    }
}
