using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SherdGroup : MonoBehaviour
{
    public List<Sherd> sherds = new();
    public float snapDistance = 0.1f;

    private Bounds _groupBounds = new Bounds();

    private void Awake()
    {
        gameObject.tag = "Draggable";

        foreach (var sherd in sherds)
        {
            _groupBounds.Encapsulate(sherd.GetComponent<Collider>().bounds);
        }
    }
    
    public bool TrySnap()
    {
        foreach (var sherd in sherds)
        {
            foreach (var rel in sherd.neighbors)
            {
                if (rel.neighbor.group == this) continue;

                Vector2 mySherdPos = new Vector2(sherd.transform.position.x, sherd.transform.position.y);
                Vector2 neighborPos = new Vector2(rel.neighbor.transform.position.x, rel.neighbor.transform.position.y);
                
                Vector2 myExpectedPosition = neighborPos - rel.offset;

                if (Vector2.Distance(mySherdPos, myExpectedPosition) < snapDistance)
                {
                    Vector2 correction = myExpectedPosition - mySherdPos;
                    transform.position += new Vector3(correction.x, correction.y, 0);
                    MergeIn(rel.neighbor.group);
                    return true;
                }
            }
        }
        return false;
    }

    public void MergeIn(SherdGroup other)
    {
        foreach (var sherd in other.sherds.ToList())
        {
            sherd.transform.SetParent(transform, worldPositionStays: true);
            sherd.group = this;
            sherds.Add(sherd);
            _groupBounds.Encapsulate(sherd.GetComponent<Collider>().bounds);
        }
        Destroy(other.gameObject);
        // UpdateBounds();
    }

    public Bounds GetGroupBounds()
    {
        if (sherds.Count == 0) return new Bounds(transform.position, Vector3.zero);
        Bounds b = sherds[0].GetComponent<Collider>().bounds;
        for (int i = 1; i < sherds.Count; i++)
            b.Encapsulate(sherds[i].GetComponent<Collider>().bounds);
        return b;
    }

    private void UpdateBounds()
    {
        foreach (var sherd in sherds)
        {
            Bounds sherdBounds = sherd.GetComponent<Collider>().bounds;
            if (sherdBounds.min.x < _groupBounds.min.x)
            {
                _groupBounds.min = sherdBounds.min;
            }

            if (sherdBounds.min.y < _groupBounds.min.y)
            {
                _groupBounds.min = sherdBounds.min;
            }
            
            if (sherdBounds.max.x > _groupBounds.max.x)
            {
                _groupBounds.max = sherdBounds.max;
            }
            
            if (sherdBounds.max.y > _groupBounds.max.y)
            {
                _groupBounds.max = sherdBounds.max;
            }
        }
    }
}
