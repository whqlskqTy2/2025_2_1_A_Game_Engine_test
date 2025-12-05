using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Dirt,
    Grass,
    Water,
    TowerSpeedUp   // 타워 공격속도 증가 아이템 추가
}

public class Block : MonoBehaviour
{
    [Header("Block Stat")]
    public ItemType type = ItemType.Dirt;
    public int maxHP = 3;
    [HideInInspector] public int hp;

    public int dropCount = 1;
    public bool mineable = true;

    void Awake()
    {
        hp = maxHP;

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
            gameObject.tag = "Block";
    }

    public void Hit(int damage, Inventory inven)
    {
        if (!mineable) return;

        hp -= damage;

        if (hp <= 0)
        {
            if (inven != null && dropCount > 0)
            {
                inven.Add(type, dropCount);

                //  여기서 UI 갱신까지 같이 호출
                var ui = FindObjectOfType<InventoryUI>();
                if (ui != null)
                {
                    ui.UpdateInventory(inven);
                }
            }

            Destroy(gameObject);
        }
    }
}