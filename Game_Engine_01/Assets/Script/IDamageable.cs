using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "피해를 받을 수 있는 대상"을 정의하는 인터페이스
public interface IDamageable
{
    // amount : 피해량
    // hitPoint : 맞은 위치
    // hitNormal : 맞은 표면 방향(이펙트 방향)
    void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitNormal);
}
