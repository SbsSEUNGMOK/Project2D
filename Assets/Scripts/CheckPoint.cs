using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static Vector3? checkPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �浹�ϸ� ���� ��ġ�� ����Ѵ�.
        // ������ ���۵Ǿ��� �� ��ϵ� ��ġ�� �ִٸ� �ش� ��ġ�� �̵��Ѵ�.
        checkPosition = transform.position;
        enabled = false;

        Debug.Log("Check point!");
    }
}
