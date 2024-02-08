using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ¿¡·¯
        Player player = collision.GetComponent<Player>();
        player.Hit();
    }
}
