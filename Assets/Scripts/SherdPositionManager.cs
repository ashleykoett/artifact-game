using System.Collections.Generic;
using UnityEngine;

public class SherdPositionManager : MonoBehaviour
{
    [SerializeField] private float radiusX;
    [SerializeField] private float radiusY;
    private Transform[] _sherds;
    private Dictionary<GameObject, Vector3> _sherdPositions = new Dictionary<GameObject, Vector3>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sherds = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform sherd in _sherds)
        {
            _sherdPositions.Add(sherd.gameObject, sherd.position);
        }
        
        ShuffleSherds();
    }

    public Vector3 GetSherdPosition(GameObject sherd)
    {
        if (_sherdPositions.ContainsKey(sherd)) 
        {
            return _sherdPositions[sherd];
        }
        return Vector3.zero;
    }

    private void ShuffleSherds()
    {
        foreach (Transform sherd in _sherds)
        {
            float x  = Random.Range(-radiusX, radiusX);
            float y = Random.Range(-radiusY, radiusY);
            sherd.position = new Vector3(x, y, sherd.position.z);
        }
    }
}
