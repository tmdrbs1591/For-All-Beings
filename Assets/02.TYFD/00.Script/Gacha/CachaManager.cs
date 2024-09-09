using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

//[System.Serializable]
//public class CharGradeInfo
//{
//    public string gradeName;
//    public Color color;
//}

public class GachaManager : MonoBehaviour
{
    public List<GachaCharacterInfo> characterInfos = new List<GachaCharacterInfo>();
    public float total = 0;

    [Header("GachaMotion")]
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject Acorn;
    [SerializeField] private Transform acornStartPos;
    [SerializeField] private Transform acornLastPos;
    [SerializeField] private Ease acornEase;
    [SerializeField] private GameObject rateCircle;
    [SerializeField] private List<CharGradeInfo> charGradeInfos = new List<CharGradeInfo>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(PlayGachaAnime(CharGrade.Common));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PlayGachaAnime(CharGrade.Rare));
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(PlayGachaAnime(CharGrade.Epic));
        }
    }


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


    IEnumerator PlayGachaAnime(CharGrade charGrade)
    {
        gachaPanel.SetActive(true);
        yield return null;

        // ���丮 ��ġ �ʱ�ȭ
        Acorn.transform.position = acornStartPos.transform.position;
        Acorn.SetActive(true);

        // x���� y���� ���� �����ϴ� �ִϸ��̼�
        Sequence sequence = DOTween.Sequence();  // DOTween�� Sequence ���

        // y���� ���� Ƣ�� �ִϸ��̼� (OutBounce ��¡ ���) + y�� ����
        float yOffset = 0.1f;  // y ��ǥ�� ���� ������
        sequence.Append(Acorn.transform
            .DOMoveY(acornLastPos.position.y - yOffset, 1f)  // ��ǥ y ��ġ�� �̵� (���� ����)
            .SetEase(Ease.OutBounce));

        // x���� �ε巴�� �̵� (���� Ƣ�� ����)
        sequence.Join(Acorn.transform
            .DOMoveZ(acornLastPos.position.z, 1f)  // ��ǥ x ��ġ�� 1�� ���� �̵�
            .SetEase(Ease.Linear));

        yield return sequence.WaitForCompletion();  // �ִϸ��̼� �Ϸ���� ���

        foreach (var gradeInfo in charGradeInfos)
        {
            if (gradeInfo.gradeName == charGrade.ToString())
            {
                rateCircle.GetComponent<Image>().color = gradeInfo.color;
            }
        }
    }
}
