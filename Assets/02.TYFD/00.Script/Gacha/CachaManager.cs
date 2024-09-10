using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    public List<GachaCharacterInfo> characterInfos = new List<GachaCharacterInfo>();
    public float total = 0;
    public List<GachaCharacterInfo> getCharaterInfos = new List<GachaCharacterInfo>();

    [Header("GachaMotion")]
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject Acorn;
    [SerializeField] private Transform acornStartPos;
    [SerializeField] private Transform acornLastPos;
    [SerializeField] private Ease acornEase;
    [SerializeField] private GameObject rateCircle;
    [SerializeField] private GameObject backgroundPanel; // ��ο� ��� �г�
    [SerializeField] private List<CharGradeInfo> charGradeInfos = new List<CharGradeInfo>();
    [SerializeField] private bool isGacha = false;
    [SerializeField] private int getCharaterIndex = 0;
    [SerializeField] private int clickCount = 0; // Ŭ�� Ƚ���� ���
    [SerializeField] private int maxClickCount = 3; // ��� ǥ�� �� �ִ� Ŭ�� Ƚ��

    [Header("Particles")]
    [SerializeField] private ParticleSystem clickParticlePrefab;  // ��ƼŬ ������

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isGacha)
        {
            HandleClick();
        }
    }

    // ĳ���͸� �����ϰ� �����ϴ� �Լ�
    public GachaCharacterInfo RandomChar()
    {
        float weight = 0;
        float selectNum = Random.Range(0f, total);

        foreach (GachaCharacterInfo c in characterInfos)
        {
            weight += c.weight;
            if (selectNum <= weight)
            {
                return c;
            }
        }

        Debug.LogError("Unexpected error: ĳ���͸� ������ �� �����ϴ�. �⺻ ĳ���͸� ��ȯ�մϴ�.");
        return characterInfos[0];
    }

    // �� �� �̱� �Լ�
    public void GachaOneTime()
    {
        getCharaterInfos.Add(RandomChar());
        Debug.Log("���õ� ĳ����: " + getCharaterInfos[0].charName);
        AcornDive();
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
                continue;
            }

            total += character.weight;
        }

        if (total == 0)
        {
            Debug.LogError("��� ĳ������ ����ġ�� 0�Դϴ�! �̱⸦ ������ �� �����ϴ�.");
        }
        else
        {
            Debug.Log("�� ����ġ: " + total);
        }
    }

    private void AcornDive()
    {
        gachaPanel.SetActive(true);

        // ���丮 ��ġ �ʱ�ȭ
        Acorn.transform.position = acornStartPos.transform.position;
        Acorn.SetActive(true);

        StartCoroutine(AcornDiveAnim());
    }

    IEnumerator AcornDiveAnim()
    {
        Sequence sequence = DOTween.Sequence();

        // y���� ���� Ƣ�� �ִϸ��̼� (OutBounce ��¡ ���) + y�� ����
        float yOffset = 0.1f;
        sequence.Append(Acorn.transform
            .DOMoveY(acornLastPos.position.y - yOffset, 1f)
            .SetEase(Ease.OutBounce));

        // x���� �ε巴�� �̵� (���� Ƣ�� ����)
        sequence.Join(Acorn.transform
            .DOMoveZ(acornLastPos.position.z, 1f)
            .SetEase(Ease.Linear));

        yield return sequence.WaitForCompletion();

        isGacha = true;
    }

    private void HandleClick()
    {
        if (clickCount < maxClickCount)
        {
            clickCount++;
            ShakeAcorn(); // Ŭ���� ������ ���丮 ����
            SpawnClickParticle(); // Ŭ�� �� ��ƼŬ ����
        }
        else
        {
            GradeCircleSizeUp(); // �ִ� Ŭ�� �� ���� �� ��� ��Ŭ ǥ��
            clickCount = 0; // Ŭ�� ī��Ʈ �ʱ�ȭ
        }
    }

    private void ShakeAcorn()
    {
        // ���� ���� ȸ�� ���� �����մϴ�.
        Vector3 currentRotation = Acorn.transform.localEulerAngles;

        // y���� �������� ��鸲 ���� ���� (�¿� ��鸲)
        float shakeAngle = 15f; // ��鸱 ���� (����)
        float shakeDuration = 0.3f; // ��鸱 �ð�

        // �¿�� ��鸮���� ���� ȸ���� �����մϴ�.
        Acorn.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y + shakeAngle, currentRotation.z), shakeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)  // �պ�
            .OnComplete(() =>
            {
                // �ִϸ��̼��� ������ ���� ������ �ǵ����ϴ�.
                Acorn.transform.localEulerAngles = currentRotation;
            });
    }

    private void SpawnClickParticle()
    {
        // Ŭ���� ��ġ�� ��ũ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Vector3 clickPosition = Input.mousePosition;
        clickPosition.z = 10f; // ī�޶󿡼��� �Ÿ� ���� (ī�޶�� Ŭ�� ��ġ ���� �Ÿ�)
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);

        // ��ƼŬ�� Ŭ�� ��ġ���� ����
        Instantiate(clickParticlePrefab, worldPosition, Quaternion.identity);
    }

    private void SetGradeCircle(CharGrade charGrade)
    {
        foreach (var gradeInfo in charGradeInfos)
        {
            if (gradeInfo.gradeName == charGrade.ToString())
            {
                rateCircle.GetComponent<Image>().color = gradeInfo.color;
            }
        }
        getCharaterIndex = 0;
        rateCircle.SetActive(true);
    }

    private void GradeCircleSizeUp()
    {
        if (getCharaterIndex > getCharaterInfos.Count)
        {
            getCharaterIndex = 0;
            return;
        }

        GachaCharacterInfo selectedCharacter = getCharaterInfos[getCharaterIndex];
        SetGradeCircle(selectedCharacter.charGrade);

        RectTransform rateCircleRect = rateCircle.GetComponent<RectTransform>();
        Image rateCircleImage = rateCircle.GetComponent<Image>();
        Image backgroundPanelImage = backgroundPanel.GetComponent<Image>();

        // ��Ŭ �ʱ� ũ�⸦ ���� (������ �� ���� ũ�⿡�� ����)
        rateCircleRect.localScale = Vector3.zero;
        rateCircleImage.color = new Color(rateCircleImage.color.r, rateCircleImage.color.g, rateCircleImage.color.b, 0);
        backgroundPanelImage.color = new Color(0, 0, 0, 0);  // ������ ������

        // ����� ��Ӱ� (1�� ����)
        backgroundPanel.SetActive(true);
        backgroundPanelImage.DOFade(0.5f, 1f);  // �������� ���������� ��ȭ

        // ��Ŭ�� ���������� ȭ���� ���� ��ŭ ũ�� Ȯ�� (2�� ����) + ������ ���� ����
        rateCircleRect.DOScale(new Vector3(10f, 10f, 10f), 2f).SetEase(Ease.OutCubic);
        rateCircleImage.DOFade(1f, 1f);
    }
}
