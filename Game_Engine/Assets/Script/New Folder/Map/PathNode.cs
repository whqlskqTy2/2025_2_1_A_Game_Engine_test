using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    // 지금은 아무 기능도 없다.
    // 나중에 Gizmo 색을 바꾸거나, 에디터 전용 로직을 넣고 싶으면 여기다 추가하면 됨.

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
#endif
}