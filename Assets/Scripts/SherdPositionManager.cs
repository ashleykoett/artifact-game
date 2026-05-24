using System.Collections.Generic;
using UnityEngine;

public class SherdPositionManager : MonoBehaviour
{
    [SerializeField] private float radiusX;
    [SerializeField] private float radiusY;

    private List<Sherd> _sherds = new();
    private Dictionary<GameObject, Vector3> _sherdPositions = new Dictionary<GameObject, Vector3>();

    private float _contactThreshold = 0.005f;
    
    void Start()
    {
        Sherd[] sherds = gameObject.GetComponentsInChildren<Sherd>();
        foreach (Sherd sherd in sherds)
        {
            _sherds.Add(sherd);
            _sherdPositions.Add(sherd.gameObject, sherd.transform.position);
            
            
            sherd.originalPosition = sherd.transform.position; // must be world pos
            Debug.Log($"{sherd.name} originalPosition: {sherd.originalPosition}");
        }
        
        BuildSherdGraph();
        // ShuffleSherds();
        Scatter();
    }
    
    private void Scatter()
    {
        foreach (var sherd in _sherds)
        {
            var groupObj = new GameObject($"Group_{sherd.sherdId}");
            groupObj.transform.position = sherd.transform.position;
            var group = groupObj.AddComponent<SherdGroup>();
            group.sherds.Add(sherd);
            sherd.transform.SetParent(groupObj.transform, worldPositionStays: true);
            sherd.group = group;

            // randomize position here
            ShuffleSherd(sherd);
        }
    }
    
    private void ShuffleSherd(Sherd sherd)
    {
        float x  = Random.Range(-radiusX, radiusX);
        float y = Random.Range(-radiusY, radiusY);
        sherd.transform.position = new Vector3(x, y, sherd.transform.position.z);
    }

    public Vector3 GetSherdPosition(Sherd sherd)
    {
        if (_sherdPositions.ContainsKey(sherd.gameObject)) 
        {
            return _sherdPositions[sherd.gameObject];
        }
        return Vector3.zero;
    }

    private void BuildSherdGraph()
    {
        float contactThreshold = 0.005f;

        for (int i = 0; i < _sherds.Count; i++)
        {
            for (int j = i + 1; j < _sherds.Count; j++)
            {
                Sherd a = _sherds[i];
                Sherd b = _sherds[j];

                if (MeshesAreInContact(a, b, contactThreshold))
                    RegisterNeighbors(a, b);
            }
        }
    }

    private bool MeshesAreInContact(Sherd a, Sherd b, float threshold)
    {
        Bounds worldBoundsA = GetWorldBounds(a);
        Bounds worldBoundsB = GetWorldBounds(b);
        worldBoundsA.Expand(threshold);
        
        if (!worldBoundsA.Intersects(worldBoundsB))
            return false;

        Vector3[] vertsA = GetWorldVertices(a);
        Vector3[] vertsB = GetWorldVertices(b);

        float threshSq = threshold * threshold;

        foreach (var va in vertsA)
        {
            foreach (var vb in vertsB)
            {
                if ((va - vb).sqrMagnitude <= threshSq)
                    return true;
            }
        }

        return false;
    }

    private void RegisterNeighbors(Sherd a, Sherd b)
    { 
        Debug.Log("Register Neighbors");
        a.neighbors.Add(new NeighborRelationship {
            neighbor = b,
            offset = new Vector2(b.originalPosition.x - a.originalPosition.x,
                b.originalPosition.y - a.originalPosition.y)
        });

        b.neighbors.Add(new NeighborRelationship {
            neighbor = a,
            offset = new Vector2(a.originalPosition.x - b.originalPosition.x,
                a.originalPosition.y - b.originalPosition.y)
        });
    }
    
    // Returns verticies of a sherd to find neighbors
    private Vector3[] GetWorldVertices(Sherd sherd)
    {
        Vector3[] local = sherd.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] world = new Vector3[local.Length];

        for (int i = 0; i < local.Length; i++)
        {
            world[i] = sherd.transform.TransformPoint(local[i]);
        }
        
        return world;
    }
    
    // Initial check to see the world space bounds of the sherd to check proximity before doing vertex calculation
    private Bounds GetWorldBounds(Sherd sherd)
    {
        Mesh mesh = sherd.GetComponent<MeshFilter>().mesh;
        
        return new Bounds(
            sherd.transform.TransformPoint(mesh.bounds.center),
            Vector3.Scale(mesh.bounds.size, sherd.transform.lossyScale)
        );
    }
}
