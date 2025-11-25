using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public PathRoute route;
    public float moveSpeed = 3f;

    private int currentIndex = 0;
    private Transform currentTarget;

    private void Start()
    {
        // 혹시 인스펙터에 안 넣었으면 자동으로 한번 찾아본다.
        if (route == null)
        {
            route = FindObjectOfType<PathRoute>();
            Debug.Log($"[EnemyMovement] route가 null이라 FindObjectOfType로 검색 → {(route ? route.name : "없음")}", this);
        }

        if (route == null)
        {
            Debug.LogError("[EnemyMovement] route가 설정되지 않았습니다. 씬에 PathRoute가 없거나 연결이 안 됐습니다.", this);
            enabled = false;
            return;
        }

        if (route.NodeCount == 0)
        {
            Debug.LogError($"[EnemyMovement] route {route.name} 에 노드가 하나도 없습니다.", this);
            enabled = false;
            return;
        }

        currentIndex = 0;
        currentTarget = route.GetNode(currentIndex);

        Debug.Log($"[EnemyMovement] Start: route = {route.name}, nodeCount = {route.NodeCount}, firstTarget = {currentTarget?.name}", this);

        // 필요하면 시작 위치를 첫 노드로 강제로 맞춰도 된다.
        // transform.position = currentTarget.position;
    }

    private void Update()
    {
        if (currentTarget == null) return;

        Vector3 targetPos = currentTarget.position;
        Vector3 dir = targetPos - transform.position;

        float distanceThisFrame = moveSpeed * Time.deltaTime;

        // 이번 프레임 안에 도착할 수 있을 만큼 가까우면 → 다음 노드로
        if (dir.magnitude <= distanceThisFrame)
        {
            NextNode();
        }
        else
        {
            transform.position += dir.normalized * distanceThisFrame;
        }
    }

    private void NextNode()
    {
        currentIndex++;

        if (currentIndex >= route.NodeCount)
        {
            Debug.Log("[EnemyMovement] 경로 끝에 도달, 적 삭제", this);
            Destroy(gameObject);
            return;
        }

        currentTarget = route.GetNode(currentIndex);
        Debug.Log($"[EnemyMovement] 다음 노드로 이동: index = {currentIndex}, target = {currentTarget?.name}", this);
    }
}