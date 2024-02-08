using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float groundRadius;
    [SerializeField] Vector3 groundOffset;

    Rigidbody2D rigid;
    bool isGrounded;
    int jumpCount;

    public bool IsGrounded => isGrounded;
    public Vector2 Veclocity => rigid.velocity;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        isGrounded = false;
        jumpCount = 0;
    }
    private void Update()
    {
        // Physics2D.OverlapCircle
        // = 특정 위치를 기준으로 r의 반지름을 가지는 원을 만들어 충돌체를 감지한다.
        
        // LayerMask : 물리적으로 충돌체를 감지할때 특정 Layer만 검색하고 싶을 때 사용한다.
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, groundMask);

        // 원에 groundMask 레이어를 가진 충돌체가 충돌했고 현재 속력 중 y값이 2이하일 경우 (=내려가는 중)
        if (isGrounded && rigid.velocity.y <= 2)
            jumpCount = 1;
    }

    public void Movement(Vector2 currentInput)
    {
        // 아래 방향키를 누르면 좌,우 움직임을 멈춰야한다. (=움직일 수 없다)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // 내 위치 값을 우측 벡터 기준 속도 * 방향 만큼 더해준다.
        //transform.position += Vector3.right * moveSpeed * hor * Time.deltaTime;        
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);
    }
    public bool Jump()
    {
        if (jumpCount <= 0)
            return false;

        // ForceMode2D.Force : 지속적인 힘, 미는 거
        // ForceMode2D.Impulse : 응축된 힘, 때리는 거
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);             // 현재 속력 중 y값을 0으로 변경.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // 위로 n의 힘만큼 힘을 가한다.
        jumpCount -= 1;                                                 // 점프 가능 횟수 1 감소.        
        return true;
    }
    public bool Throw(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse);
        return true;
    }
    public void SwitchFreeze(bool isOn)
    {
        rigid.bodyType = isOn ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
    }

    // 기즈모(Gizmos)
    // = 디버깅 용도로 그리는 도형 (실제 게임에서는 보이지 않는다)
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position + groundOffset, Vector3.back, groundRadius);
    }
}

// Ctrl+T : 검색 윈도우 (클래스, 메서드, 필드 등을 찾을 수 있다)
// Ctrl+F : 검색창 (원하는 문자열을 검색할 수 있다)
// Ctrl+H : 대체창 (특정 문자를 원하는 문자로 변경할 수 있다)
// Ctrl+S : 저장

// C+Sh+F : 모든 파일 검색
// C+Sh+H : 모든 문자열 일괄 변경
// C+Sh+S : 모든 파일 일괄 저장
// C+Sh+V : 최근 10개까지 복사한 문자열 붙여넣기

// Alt+위,아래 : 한 줄 위 아래로 옮기기
// Alt+Shift+위,아래 : 멀티 라인 선택
// Shift+Del : 한 줄 지우기
