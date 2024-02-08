using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ITEM
{
    GEM,
    CHERRY,
}

public class Item : MonoBehaviour
{
    [SerializeField] ITEM id;
    [SerializeField] int count;

    // Item ��ü�� Item Layer�� ������ �ִ�.
    // �ش� ���̾�� ���� Player Layer�� �浹 ó���ȴ�.
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (count <= 0)
            count = 1;

        Player player = collision.GetComponent<Player>();
        player.GetItem(id, count);
        GetComponent<Animator>().SetTrigger("on");
    }
}
