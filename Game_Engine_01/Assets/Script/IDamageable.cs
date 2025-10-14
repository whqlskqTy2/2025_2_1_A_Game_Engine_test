using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "���ظ� ���� �� �ִ� ���"�� �����ϴ� �������̽�
public interface IDamageable
{
    // amount : ���ط�
    // hitPoint : ���� ��ġ
    // hitNormal : ���� ǥ�� ����(����Ʈ ����)
    void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitNormal);
}
