using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SherdGroup : MonoBehaviour
{
    public List<Sherd> sherds = new();
    public float snapDistance = 0.5f;

    // Returns true if a snap occurred (so caller can re-check)
    public bool TrySnap()
    {
        foreach (var shard in sherds)
        {
            foreach (var rel in shard.neighbors)
            {
                if (rel.neighbor.group == this) continue;

                Vector2 myShardPos = new Vector2(shard.transform.position.x, shard.transform.position.y);
                Vector2 neighborPos = new Vector2(rel.neighbor.transform.position.x, rel.neighbor.transform.position.y);
                Vector2 expectedNeighborPos = myShardPos + rel.offset;

                if (Vector2.Distance(neighborPos, expectedNeighborPos) < snapDistance)
                {
                    Vector2 correction = expectedNeighborPos - neighborPos;
                    SherdGroup otherGroup = rel.neighbor.group;
                    otherGroup.transform.position += new Vector3(correction.x, correction.y, 0);
                    
                    Debug.Log($"Sherd: {shard.name} at {myShardPos}");
                    Debug.Log($"Expected neighbor pos: {expectedNeighborPos}");
                    Debug.Log($"Neighbor actual pos: {neighborPos}");
                    Debug.Log($"Correction: {correction}");
                    MergeIn(otherGroup);
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
