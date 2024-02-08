using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] Movement2D movement2D;

    void Update()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Enemy");
        Collider2D check = Physics2D.OverlapCircle(transform.position, radius, mask);
        if(check != null)
        {
            check.GetComponent<Enemy>().Hit();
            movement2D.Throw(6f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, radius);
    }
}
