using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    Toggle[] hpToggles;
    
    public void SetupHp(int value)
    {
        if (hpToggles == null)
            hpToggles = transform.GetComponentsInChildren<Toggle>();

        // ������� ��û���� �غ�� UI�� �� ���� ��� ���� ����.
        if(hpToggles.Length < value)
        {
            int addCount = value - hpToggles.Length;
            Toggle preset = hpToggles[0];
            for (int i = 0; i < addCount; i++)
                Instantiate(preset, transform);

            hpToggles = transform.GetComponentsInChildren<Toggle>();
        }

        for(int i = 0; i<hpToggles.Length; i++)
            hpToggles[i].gameObject.SetActive(i < value);
    }
    public void UpdateHP(int value)
    {
        for(int i = 0; i<hpToggles.Length; i++)
            hpToggles[i].isOn = i < value;
    }
}