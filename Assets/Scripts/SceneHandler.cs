using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHandler
{
    public enum ID
    {
        Title,
        Main,
        Game,
        Credit,
    }
    public static ID current { get; private set; }

    // static 클래스의 생성자는 해당 객체에게 참조되기 전 1회 호출된다.
    // public이 될 수 없다. static 객체는 사용자가 임의로 만드는 것이 아니다.
    static SceneHandler()
    {
        Scene scene = SceneManager.GetActiveScene();
        ID id = 0;
        System.Enum.TryParse(scene.name, out id);   // 열거형 파싱.
        current = id;
    }

    public static void LoadScene(ID id)
    {
        current = id;
        SceneManager.LoadScene(id.ToString());
    }
}