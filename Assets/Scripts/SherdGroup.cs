using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SherdGroup : MonoBehaviour
{
    public List<Sherd> sherds = new();
    public float snapDistance = 0.1f;
    
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

    void MergeIn(SherdGroup other)
    {
        foreach (var sherd in other.sherds.ToList())
        {
            sherd.transform.SetParent(transform, worldPositionStays: true);
            sherd.group = this;
            sherds.Add(sherd);
        }
        Destroy(other.gameObject);
    }
}
