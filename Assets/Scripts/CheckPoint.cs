using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static Vector3? checkPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌하면 현재 위치를 기록한다.
        // 게임이 시작되었을 때 기록된 위치가 있다면 해당 위치로 이동한다.
        checkPosition = transform.position;
        enabled = false;

        Debug.Log("Check point!");
    }
}
