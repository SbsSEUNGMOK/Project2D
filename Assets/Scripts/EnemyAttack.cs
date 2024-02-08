using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //  충돌체      영역체크    충돌체크
    // Collider       O           O
    // Trigger        O           X

    // 내가 Trigger이기 때문에 Collision(충돌)처리를 할 수 없다.
    // 따라서 상대 오브젝트가 Collider라고 해도 Trigger체크만 가능하다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Player>().Hit();
    }
}

