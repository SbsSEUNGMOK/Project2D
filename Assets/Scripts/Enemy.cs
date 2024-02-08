using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Movement2D movement2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] bool isLeft;

    CapsuleCollider2D collider2D;

    LayerMask groundMask;
    Vector3 rayHeight;    
    Vector3 rayPoint;

    private void Start()
    {
        // 게임 시작 시 변화하지 않는 값을 미리 계산해 캐싱
        groundMask = 1 << LayerMask.NameToLayer("Ground");
        rayHeight = Vector3.up * 0.5f;
        collider2D = GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {
        Raycast();
        Movement();
    }

    void Raycast()
    {
        // 정면 오브젝트 체크 : 레이에 충돌하면 방향을 돌린다.
        float dir = isLeft ? -1f : 1f;                                                                                  // 방향.
        Vector3 point = transform.position + (Vector3.right * 0.5f * dir) + (Vector3.up * collider2D.size.y);           // ray의 시작점
        if(Physics2D.Raycast(point, Vector3.down, collider2D.size.y * 0.9f, groundMask))                                       // raycast (충돌시 true)
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
            dir = isLeft ? -1f : 1f;
        }

        // 절벽 체크 : 만약 레이에 충돌하지 않으면 정면에 절벽이 있다고 판단해 방향을 뒤집는다.
        rayPoint = transform.position + (Vector3.right * 0.5f * dir) + rayHeight;
        if (!Physics2D.Raycast(rayPoint, Vector2.down, 1f, groundMask))
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
        }
    }
    void Movement()
    {
        // 방향에 따른 입력 값.
        Vector2 input = new Vector2(isLeft ? -1f : 1f, 0f);
        movement2D.Movement(input);
    }

    public void Hit()
    {
        collider2D.enabled = false;
        enabled = false;
        GetComponent<Animator>().SetTrigger("onDead");
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayPoint, Vector3.down);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Vector3 point = transform.position + Vector3.right * 0.5f * (isLeft ? -1f : 1f) + Vector3.up * collider2D.size.y;
            Gizmos.DrawRay(point, Vector3.down * (collider2D.size.y * 0.9f));
        }
    }
}
