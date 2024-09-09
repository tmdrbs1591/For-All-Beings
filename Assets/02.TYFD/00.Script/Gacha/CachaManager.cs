using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public List<GachaCharacterInfo> characterInfos = new List<GachaCharacterInfo>();
    public float total = 0;

    // ĳ���͸� �����ϰ� �����ϴ� �Լ�
    public GachaCharacterInfo RandomChar()
    {
        float weight = 0;
        float selectNum = Random.Range(0f, total); // 1���� total���� ���� ����

        //Debug.Log("�������� ���õ� ����: " + selectNum); // ���� ���� �� Ȯ��
        foreach (GachaCharacterInfo c in characterInfos)
        {
            weight += c.weight;
            //Debug.Log(c.charName + " ĳ������ ���� ����ġ: " + weight); // ĳ���� ����ġ Ȯ��

            // ���⼭ ���õ� ĳ���͸� ��ȯ�մϴ�.
            if (selectNum <= weight)
            {
                //Debug.Log("���õ� ĳ����: " + c.charName);
                return c;
            }
        }

        // �� �������� ĳ���Ͱ� ������ ���õǾ�� �ϹǷ� ����� ���� �������� �ʽ��ϴ�.
        // ���� ����ȴٸ�, �̴� ����ġ ���� ���װ� �ִٴ� �ǹ��Դϴ�.
        Debug.LogError("Unexpected error: ĳ���͸� ������ �� �����ϴ�. �⺻ ĳ���͸� ��ȯ�մϴ�.");
        return characterInfos[0]; // �⺻ ĳ���� ��ȯ (�� �κ��� ���� ������� ���� ��)
    }


    // �� �� �̱� �Լ�
    public void GachaOneTime()
    {
        GachaCharacterInfo character = RandomChar();
        Debug.Log("���õ� ĳ����: " + character.charName); // ���õ� ĳ���� �̸� ���
    }

    // 10�� �̱� �Լ�
    public void GachaTenTime()
    {
        for (int i = 0; i < 10; i++)
        {
            GachaOneTime();
        }
    }

    // �� ����ġ ��� �� �ʱ�ȭ
    private void Start()
    {
        // ĳ���� ������ ���� ��� ����
        if (characterInfos == null || characterInfos.Count == 0)
        {
            Debug.LogError("ĳ���� ���� ����Ʈ�� ����ֽ��ϴ�! �̱⸦ ������ �� �����ϴ�.");
            return;
        }

        total = 0;
        foreach (var character in characterInfos)
        {
            if (character.weight <= 0)
            {
                Debug.LogError("����ġ�� 0 ������ ĳ���Ͱ� �ֽ��ϴ�: " + character.charName);
                continue; // ����ġ�� 0 ������ ĳ���ʹ� ����
            }

            total += character.weight;
        }

        // �� ����ġ�� 0�� ��� ����
        if (total == 0)
        {
            Debug.LogError("��� ĳ������ ����ġ�� 0�Դϴ�! �̱⸦ ������ �� �����ϴ�.");
        }
        else
        {
            Debug.Log("�� ����ġ: " + total); // �� ����ġ Ȯ��
        }
    }
}
