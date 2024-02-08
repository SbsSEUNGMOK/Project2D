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

    // Item 객체는 Item Layer를 가지고 있다.
    // 해당 레이어는 오직 Player Layer와 충돌 처리된다.
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (count <= 0)
            count = 1;

        Player player = collision.GetComponent<Player>();
        player.GetItem(id, count);
        GetComponent<Animator>().SetTrigger("on");
    }
}
