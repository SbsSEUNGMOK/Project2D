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

    Animator anim;                      // �ִϸ�����.
    Vector2 currentInput;               // ���� �Է� ��.
    LayerMask enemyMask;                // ���ʹ� ���̾� ����ũ.
    SpriteRenderer spriteRenderer;      // ��������Ʈ ������.
    Dictionary<ITEM, int> itemCount;    // �� ITEM�� ����.
    bool isGameClear;                   // ���� Ŭ���� ����.
    int hp;                             // ���� ü��.

    void Start()
    {
        anim = GetComponent<Animator>();
        currentInput = Vector2.zero;
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCount = new Dictionary<ITEM, int>();
        hp = maxHp;
        hpUI.SetupHp(hp);

        // üũ����Ʈ �����Ͱ� �ִٸ� ��ġ�� �����Ѵ�.
        if (CheckPoint.checkPosition != null)
            transform.position = (Vector3)CheckPoint.checkPosition;
    }
    private void Update()
    {
        if (isGameClear || hp <= 0)
            return;

        // Rigidbody�� �̿��� �̵� ó������ �� ���� ��������� �����Ѵ�.
        // 1.���� : �⺻ ���Դ� 1�̴�.
        // 2.���� : ��������, ȸ�������� ������ �� �ִ�.
        // 3.������ : �浹ü�� ���˸鿡 �������� ��������.
        // Ű���� ��,��Ű�� -1, 0, 1�� ������ ����.
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");
        movement2D.Movement(currentInput);

        // ����) ���� Ű ������ ���� ���� ĳ���Ͱ� �������� ����.
        //  - ����Ű�� ������ hor�� -1�� ������� ���ǽ��� true�� �Ǿ��� ������ ������ �ٶ󺸾�����
        //  - ���� ���� hor�� 0�̵ǰ� �ٽ� ���ǽ��� �������� �ʰ� �ȴ�.
        // �ᱹ)
        //  - ���� Ű �Է��� �Ҷ��� filpX ���� �ٲ۴�.
        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;

        // ���� �Է�Ű�� ���� Jump�� ȣ���Ѵ�.
        // ���� ���� ���δ� Movement2D�� �Ǵ��Ѵ�.
        if (Input.GetKeyDown(KeyCode.Z) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // �� �ؿ� ���� �浹�� ��� �������� �ش�.
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, attackRadius, enemyMask);
        if(enemyCollider != null && movement2D.Veclocity.y < 0)
        {
            enemyCollider.GetComponent<Enemy>().Hit();
            movement2D.Throw(7f);
        }

    }
    void LateUpdate()
    {
        // �ִϸ������� �Ķ���� ����.
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

    // �ܺ� �Լ�
    public void GetItem(ITEM id, int count)
    {
        if (!itemCount.ContainsKey(id))     // ��ųʸ��� �ش��ϴ� Ű�� ���ٸ�.
            itemCount.Add(id, 0);           // ���ο� ������ �߰�.

        itemCount[id] += count;             // id�� �ش��ϴ� �����Ϳ� �� ���ϱ�.
    }
    public void Hit()
    {
        // �������� ��¦ ���� hurt �ִϸ��̼��� �����Ѵ�.
        // �� �ִϸ��̼��� ���� �����ϰų� 1 cycle�� ���� ������.
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

    // ���� ���´� �Ͻ����̱� ������ �ڷ�ƾ�� �̿��ϸ� ���ϴ�.
    // ���� �ð��� n�� ���� ĳ���Ͱ� ��¦�δ�.
    IEnumerator IEGodMode()
    {
        float godModeTime = 2.0f;               // ���� �ð�.
        float offset = 0.05f;                   // ��¦�̴� �ð� ����.
        float time = offset;                    // ��¦�̴� �ð�.
        int prevLayer = gameObject.layer;       // �±� �� ĳ��.

        gameObject.layer = LayerMask.NameToLayer("GodMode");            // �±� ����
        spriteRenderer.ChangeAlpha(0.8f);                               // Ȯ�� �޼��带 �̿��� ���İ� 80%�� ����

        while ((godModeTime -= Time.deltaTime) >= 0.0f)                  // �ð� �� ����
        {
            if ((time -= Time.deltaTime) <= 0.0f)                       // �ð� �� ����
            {
                time = offset;                                          // time���� offset�� ����.
                spriteRenderer.enabled = !spriteRenderer.enabled;       // �������� ���� ���� �ݴ�� ����.
            }
            yield return null;
        }

        spriteRenderer.enabled = true;          // ������ Ȱ��ȭ.
        spriteRenderer.ChangeAlpha(1f);         // (Ȯ�� �޼���) ���� �� 100%�� ����.
        gameObject.layer = prevLayer;           // ���� �±׷� �ǵ�����.

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
 *  �� Component
 *    �� Behaviour
 *       �� MonoBehaviour
 */

// Ȯ�� �޼���� static Ŭ���� ���ο� ��������Ѵ�. (Ŭ������ �������)
public static class Method
{
    public static void ChangeAlpha(this SpriteRenderer target, float alpha)
    {
        Color color = target.color;
        color.a = alpha;
        target.color = color;
    }
}

