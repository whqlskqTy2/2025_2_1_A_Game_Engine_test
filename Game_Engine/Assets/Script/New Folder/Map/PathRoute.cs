using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRoute : MonoBehaviour
{
    // 인스펙터에서 직접 넣는 노드 리스트
    public List<Transform> nodes = new List<Transform>();

    public Transform GetNode(int index)
    {
        if (index < 0 || index >= nodes.Count) return null;
        return nodes[index];
    }

    public int NodeCount => nodes.Count;

    private void OnDrawGizmos()
    {
        if (nodes == null || nodes.Count == 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < nodes.Count; i++)
        {
            Transform node = nodes[i];
            if (node == null) continue;

            Gizmos.DrawSphere(node.position, 0.2f);

            if (i > 0 && nodes[i - 1] != null)
            {
                Gizmos.DrawLine(nodes[i - 1].position, node.position);
            }
        }
    }
}