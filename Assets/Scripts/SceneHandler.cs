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

    // static Ŭ������ �����ڴ� �ش� ��ü���� �����Ǳ� �� 1ȸ ȣ��ȴ�.
    // public�� �� �� ����. static ��ü�� ����ڰ� ���Ƿ� ����� ���� �ƴϴ�.
    static SceneHandler()
    {
        Scene scene = SceneManager.GetActiveScene();
        ID id = 0;
        System.Enum.TryParse(scene.name, out id);   // ������ �Ľ�.
        current = id;
    }

    public static void LoadScene(ID id)
    {
        current = id;
        SceneManager.LoadScene(id.ToString());
    }
}