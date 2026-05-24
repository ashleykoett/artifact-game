using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeighborRelationship
{
    public Sherd neighbor;
    public Vector2 offset;
}

public class Sherd : MonoBehaviour
{
    public int sherdId;
    public Vector2 originalPosition;
    public List<NeighborRelationship> neighbors = new();
    public SherdGroup group;

    /*
    public void CheckSnaps()
    {
        Debug.Log("Checking Snaps");
        foreach (var rel in neighbors)
        {
            // if (rel.neighbor.isAnchored) continue; // already placed, skip

            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 neighborPos = new Vector2(rel.neighbor.transform.position.x,
                rel.neighbor.transform.position.y);

            if (rel.neighbor.isAnchored)
            {
                // Snap me into correct position relative to already-anchored neighbor
                Vector2 myTargetPos = neighborPos - rel.offset;

                if (Vector2.Distance(myPos, myTargetPos) < snapDistance)
                {
                    transform.position = new Vector3(myTargetPos.x, myTargetPos.y, transform.position.z);
                    isAnchored = true;
                    return;
                }
            }
            else
            {
                // First match — anchor me here, snap neighbor to correct relative position
                Vector2 expectedNeighborPos = myPos + rel.offset;

                if (Vector2.Distance(neighborPos, expectedNeighborPos) < snapDistance)
                {
                    rel.neighbor.transform.position = new Vector3(expectedNeighborPos.x, expectedNeighborPos.y,
                        rel.neighbor.transform.position.z);
                    rel.neighbor.isAnchored = true;
                    isAnchored = true;
                    return;
                }
            }
        }
    }
    */
}
