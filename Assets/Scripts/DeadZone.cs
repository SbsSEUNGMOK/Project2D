using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            // nameof() : 변수, 메서드, 클래스명을 string으로 변경

            player.gameObject.SetActive(false);     // 플레이어 오브젝트 비활성화.
            Invoke(nameof(ReloadScene), 1.5f);      // 지연 실행
        }
    }

    private void ReloadScene()
    {
        SceneHandler.LoadScene(SceneHandler.ID.Game);
    }
}
