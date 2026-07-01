using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    public Vector3 offset;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    private Vector3 _targetPosition;

    void Update() {
        _targetPosition = target.transform.position + offset;
        _targetPosition.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref velocity, smoothTime);
    }
}
