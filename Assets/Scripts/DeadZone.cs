using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            // nameof() : ����, �޼���, Ŭ�������� string���� ����

            player.gameObject.SetActive(false);     // �÷��̾� ������Ʈ ��Ȱ��ȭ.
            Invoke(nameof(ReloadScene), 1.5f);      // ���� ����
        }
    }

    private void ReloadScene()
    {
        SceneHandler.LoadScene(SceneHandler.ID.Game);
    }
}
