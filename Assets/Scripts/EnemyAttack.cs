using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //  �浹ü      ����üũ    �浹üũ
    // Collider       O           O
    // Trigger        O           X

    // ���� Trigger�̱� ������ Collision(�浹)ó���� �� �� ����.
    // ���� ��� ������Ʈ�� Collider��� �ص� Triggerüũ�� �����ϴ�.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Player>().Hit();
    }
}

