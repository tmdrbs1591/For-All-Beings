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
    [SerializeField] private GameObject acorn;
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
    [SerializeField] private Material acronDefultMaterial;
    [SerializeField] private Material acornBlurMaterial;

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
        acorn.GetComponentInChildren<Renderer>().material = acronDefultMaterial;
        Debug.Log("���õ� ĳ����: " + getCharaterInfos[0].charName);
        SetGradeCircle(getCharaterInfos[0].charGrade);
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
        acorn.transform.position = acornStartPos.transform.position;
        acorn.SetActive(true);

        StartCoroutine(AcornDiveAnim());
    }

    IEnumerator AcornDiveAnim()
    {
        Sequence sequence = DOTween.Sequence();

        // y���� ���� Ƣ�� �ִϸ��̼� (OutBounce ��¡ ���) + y�� ����
        float yOffset = 0.1f;
        sequence.Append(acorn.transform
            .DOMoveY(acornLastPos.position.y - yOffset, 1f)
            .SetEase(Ease.OutBounce));

        // x���� �ε巴�� �̵� (���� Ƣ�� ����)
        sequence.Join(acorn.transform
            .DOMoveZ(acornLastPos.position.z, 1f)
            .SetEase(Ease.Linear));

        yield return sequence.WaitForCompletion();

        isGacha = true;
    }

    private void HandleClick()
    {
        // �̹� �ִ� Ŭ�� ���� �����ߴٸ� �� �̻� ó������ ����
        if (clickCount >= maxClickCount)
        {
            return;
        }

        clickCount++;
        ShakeAcorn(); // Ŭ���� ������ ���丮 ����
        SpawnClickParticle(); // Ŭ�� �� ��ƼŬ ����

        // Ŭ�� Ƚ���� ���� ���丮 ���� ����
        ChangeAcornColorByClickCount();

        // �ִ� Ŭ�� ���� �����ϸ� ��� ��Ŭ ǥ�� �� Ŭ�� ����
        if (clickCount == maxClickCount)
        {
            GradeCircleSizeUp(); // ��� ��Ŭ�� ũ�� ����
            isGacha = false; // �� �̻� Ŭ���� �� ������ ����
        }
    }

    private void ChangeAcornColorByClickCount()
    {
        // ���� ĳ������ ��� ����
        CharGrade finalGrade = getCharaterInfos[getCharaterIndex].charGrade;

        // 1��° Ŭ�������� ������ Ŀ�� ����
        if (clickCount == 1)
        {
            SetAcornColor(CharGrade.Common);
        }
        // 2��° Ŭ�������� ���� �̻��� ��� ���� ����
        else if (clickCount == 2)
        {
            if (finalGrade == CharGrade.Rare || finalGrade == CharGrade.Epic)
            {
                SetAcornColor(CharGrade.Rare);
            }
            else
            {
                SetAcornColor(CharGrade.Common);
            }
        }
        // 3��° Ŭ�������� ���� ĳ������ ���� ��� ����
        else if (clickCount == 3)
        {
            SetAcornColor(finalGrade);
            //isGacha = false;
        }
    }

    private void ShakeAcorn()
    {
        // ���� ���� ȸ�� ���� �����մϴ�.
        Vector3 currentRotation = acorn.transform.localEulerAngles;

        // y���� �������� ��鸲 ���� ���� (�¿� ��鸲)
        float shakeAngle = 15f; // ��鸱 ���� (����)
        float shakeDuration = 0.3f; // ��鸱 �ð�

        // �¿�� ��鸮���� ���� ȸ���� �����մϴ�.
        acorn.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y + shakeAngle, currentRotation.z), shakeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)  // �պ�
            .OnComplete(() =>
            {
                // �ִϸ��̼��� ������ ���� ������ �ǵ����ϴ�.
                acorn.transform.localEulerAngles = currentRotation;
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

    private void SetAcornColor(CharGrade charGrade)
    {
        foreach (var gradeInfo in charGradeInfos)
        {
            if (gradeInfo.gradeName == charGrade.ToString())
            {
                MeshRenderer meshRenderer = acorn.GetComponentInChildren<MeshRenderer>();

                meshRenderer.material = acornBlurMaterial;

                if (meshRenderer != null)
                {
                    Material material = meshRenderer.material;
                    if (material.HasProperty("_GlowColor"))
                    {
                        material.SetColor("_GlowColor", gradeInfo.color);
                    }
                }
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
        backgroundPanelImage.color = new Color(0, 0, 0, 0); // ��ο� �г� ����

        rateCircleRect.DOScale(8f, 0.4f).SetEase(Ease.OutBack); // ��Ŭ ũ�� �ִϸ��̼�
        DOTween.ToAlpha(() => rateCircleImage.color, color => rateCircleImage.color = color, 1f, 0.2f); // ��Ŭ ���İ� �ִϸ��̼�
        DOTween.ToAlpha(() => backgroundPanelImage.color, color => backgroundPanelImage.color = color, 0.6f, 0.3f); // ��ο� ��� ���� �ִϸ��̼�
    }
}