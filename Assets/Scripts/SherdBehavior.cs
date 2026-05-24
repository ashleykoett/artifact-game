using System;
using UnityEngine;

public class SherdBehavior : MonoBehaviour
{
    [SerializeField] private float snapRadius = 0.1f;
    private SherdPositionManager _sherdPositionManager;
    private bool _selected  = false;

    private void Start()
    {
        _sherdPositionManager = GetComponentInParent<SherdPositionManager>();
    }

    public void OnSelected()
    {
        _selected = true;
    }

    public void Move(Vector3 position)
    {
        transform.position = position;
        if (_sherdPositionManager != null)
        {
            // snap when close
        }
    }
}
