using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Movement2D movement2D;
    [SerializeField] FollowCamera followCam;
    [SerializeField] HpUI hpUI;
    [SerializeField] float attackRadius;
    [SerializeField] int maxHp;

    Animator anim;                      // 애니메이터.
    Vector2 currentInput;               // 현재 입력 값.
    LayerMask enemyMask;                // 에너미 레이어 마스크.
    SpriteRenderer spriteRenderer;      // 스프라이트 렌더러.
    Dictionary<ITEM, int> itemCount;    // 각 ITEM별 개수.
    bool isGameClear;                   // 게임 클리어 상태.
    int hp;                             // 현재 체력.

    void Start()
    {
        anim = GetComponent<Animator>();
        currentInput = Vector2.zero;
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCount = new Dictionary<ITEM, int>();
        hp = maxHp;
        hpUI.SetupHp(hp);

        // 체크포인트 데이터가 있다면 위치를 복구한다.
        if (CheckPoint.checkPosition != null)
            transform.position = (Vector3)CheckPoint.checkPosition;
    }
    private void Update()
    {
        if (isGameClear || hp <= 0)
            return;

        // Rigidbody를 이용한 이동 처리에는 몇 가지 고려사항이 존재한다.
        // 1.질량 : 기본 무게는 1이다.
        // 2.저항 : 공기저항, 회전저항이 존재할 수 있다.
        // 3.마찰력 : 충돌체의 접촉면에 마찰력이 가해진다.
        // 키보드 좌,우키를 -1, 0, 1의 값으로 리턴.
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");
        movement2D.Movement(currentInput);

        // 문제) 왼쪽 키 누르고 손을 때면 캐릭터가 오른쪽을 본다.
        //  - 왼쪽키를 눌러서 hor을 -1로 만들었고 조건식이 true가 되었기 때문에 왼쪽을 바라보았지만
        //  - 손을 때면 hor이 0이되고 다시 조건식을 만족하지 않게 된다.
        // 결국)
        //  - 내가 키 입력을 할때만 filpX 값을 바꾼다.
        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;

        // 점프 입력키를 눌러 Jump를 호출한다.
        // 점프 가능 여부는 Movement2D가 판단한다.
        if (Input.GetKeyDown(KeyCode.Z) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // 발 밑에 적이 충돌할 경우 데미지를 준다.
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, attackRadius, enemyMask);
        if(enemyCollider != null && movement2D.Veclocity.y < 0)
        {
            enemyCollider.GetComponent<Enemy>().Hit();
            movement2D.Throw(7f);
        }

    }
    void LateUpdate()
    {
        // 애니메이터의 파라미터 갱신.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isCrouch", currentInput.y == -1);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Veclocity.y));

        currentInput = Vector2.zero;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Goal")
        {
            isGameClear = true;
            movement2D.Movement(Vector2.zero);
            CheckPoint.checkPosition = null;
        }
    }

    // 외부 함수
    public void GetItem(ITEM id, int count)
    {
        if (!itemCount.ContainsKey(id))     // 딕셔너리에 해당하는 키가 없다면.
            itemCount.Add(id, 0);           // 새로운 데이터 추가.

        itemCount[id] += count;             // id에 해당하는 데이터에 값 더하기.
    }
    public void Hit()
    {
        // 공중으로 살짝 띄우고 hurt 애니메이션을 실행한다.
        // 이 애니메이션은 땅에 도착하거나 1 cycle이 돌면 끝난다.
        if (movement2D.Throw(8f))
        {
            hp -= 1;
            hpUI.UpdateHP(hp);

            if (hp <= 0)
            {
                StartCoroutine(IEDead());
            }
            else
            {
                anim.SetTrigger("onHurt");
                StartCoroutine(IEGodMode());
            }
        }
    }
    IEnumerator IEDead()
    {
        anim.SetTrigger("onDead");
        followCam.enabled = false;
        movement2D.Movement(Vector2.zero);
        movement2D.SwitchFreeze(true);
        yield return new WaitForSeconds(1f);

        movement2D.SwitchFreeze(false);
        movement2D.Throw(8f);
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        SceneHandler.LoadScene(SceneHandler.ID.Game);
    }

    // 무적 상태는 일시적이기 때문에 코루틴을 이용하면 편리하다.
    // 유지 시간은 n초 동안 캐릭터가 반짝인다.
    IEnumerator IEGodMode()
    {
        float godModeTime = 2.0f;               // 지속 시간.
        float offset = 0.05f;                   // 반짝이는 시간 간격.
        float time = offset;                    // 반짝이는 시간.
        int prevLayer = gameObject.layer;       // 태그 값 캐싱.

        gameObject.layer = LayerMask.NameToLayer("GodMode");            // 태그 변경
        spriteRenderer.ChangeAlpha(0.8f);                               // 확장 메서드를 이용해 알파값 80%로 조절

        while ((godModeTime -= Time.deltaTime) >= 0.0f)                  // 시간 값 감소
        {
            if ((time -= Time.deltaTime) <= 0.0f)                       // 시간 값 감소
            {
                time = offset;                                          // time값을 offset로 대입.
                spriteRenderer.enabled = !spriteRenderer.enabled;       // 렌더러의 상태 값을 반대로 대입.
            }
            yield return null;
        }

        spriteRenderer.enabled = true;          // 렌더러 활성화.
        spriteRenderer.ChangeAlpha(1f);         // (확장 메서드) 알파 값 100%로 조절.
        gameObject.layer = prevLayer;           // 이전 태그로 되돌린다.

        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        yield return null;
        collider.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRadius);
    }
}

/*
 * Object
 *  ㄴ Component
 *    ㄴ Behaviour
 *       ㄴ MonoBehaviour
 */

// 확장 메서드는 static 클래스 내부에 존재햐야한다. (클래스명 상관없음)
public static class Method
{
    public static void ChangeAlpha(this SpriteRenderer target, float alpha)
    {
        Color color = target.color;
        color.a = alpha;
        target.color = color;
    }
}

