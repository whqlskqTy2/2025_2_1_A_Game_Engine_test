using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("설치할 타워 프리팹")]
    public GameObject towerPrefab;

    [Header("상태")]
    public bool isOccupied = false;
    public GameObject currentTower;

    public void PlaceTower(GameObject prefab)
    {
        if (isOccupied)
        {
            Debug.Log("이미 타워가 설치된 슬롯입니다.", this);
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning("타워 프리팹이 비어 있습니다.", this);
            return;
        }

        currentTower = Instantiate(prefab, transform.position, Quaternion.identity);
        isOccupied = true;
    }

    private void OnMouseDown()
    {
        if (isOccupied)
        {
            Debug.Log("이 슬롯에는 이미 타워가 있습니다.", this);
            return;
        }

        if (towerPrefab == null)
        {
            Debug.LogWarning("TowerSlot에 towerPrefab이 지정되지 않았습니다.", this);
            return;
        }

        PlaceTower(towerPrefab);
        Debug.Log("타워 설치 완료", this);
    }
}