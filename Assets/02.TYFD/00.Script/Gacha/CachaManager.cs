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
    [SerializeField] private bool isResult = false;
    [SerializeField] private int getCharaterIndex = 0;
    [SerializeField] private int clickCount = 0; // Ŭ�� Ƚ���� ���
    [SerializeField] private int maxClickCount = 3; // ��� ǥ�� �� �ִ� Ŭ�� Ƚ��
    [SerializeField] private Material acronDefultMaterial;
    [SerializeField] private Material acornBlurMaterial;

    [Header("Particles")]
    [SerializeField] private ParticleSystem clickParticlePrefab;  // ��ƼŬ ������

    [Header("ResultChar")]
    [SerializeField] private GameObject resultPanelObj;
    [SerializeField] private Transform gridParent; 
    [SerializeField] private List<GameObject> resultCharImages = new List<GameObject>();
    [SerializeField] private CharGrade bestGrade = CharGrade.Common;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isGacha)
        {
            HandleClick();
        }
        if (Input.GetMouseButtonDown(0) && isResult)
        {
            InitGacha();
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

    public void PlayGacha()
    {
        getCharaterInfos.Add(RandomChar());
        acorn.GetComponentInChildren<Renderer>().material = acronDefultMaterial;
    }

    // �� �� �̱� �Լ�
    public void GachaOneTime()
    {
        AcornDive();
        PlayGacha();
    }

    // 10�� �̱� �Լ�
    public void GachaTenTime()
    {
        AcornDive();

        for (int i = 0; i < 10; i++)
        {
            PlayGacha();
        }

        foreach(GachaCharacterInfo c in characterInfos)
        {
            if(c.charGrade < bestGrade)
            {
                bestGrade = c.charGrade;
            }
        }
    }

    IEnumerator ResultCharaterImageSpawn()
    {
        if (getCharaterInfos.Count == 1)
        {
            // 1ȸ �̱��� �� �׸��� ���̾ƿ� ��Ȱ��ȭ
            gridParent.GetComponent<GridLayoutGroup>().enabled = false;

            // ĳ���� �̹����� �߾ӿ� ��ġ (���� ��ġ)
            GameObject charImage = Instantiate(getCharaterInfos[0].charImage, gridParent);
            RectTransform charRect = charImage.GetComponent<RectTransform>();

            // �߾� ��ġ�� ���� ��Ŀ�� ��ġ ����
            charRect.anchorMin = new Vector2(0.5f, 0.5f);
            charRect.anchorMax = new Vector2(0.5f, 0.5f);
            charRect.pivot = new Vector2(0.5f, 0.5f);
            charRect.anchoredPosition = Vector2.zero; // �߾ӿ� ��ġ
            resultCharImages.Add(charImage);
        }
        else if (getCharaterInfos.Count > 1)
        {
            // 10ȸ �̱��� �� �׸��� ���̾ƿ� Ȱ��ȭ
            gridParent.GetComponent<GridLayoutGroup>().enabled = true;

            // ĳ���� �̹����� ���ʴ�� �׸��忡 ����
            foreach (GachaCharacterInfo c in getCharaterInfos)
            {
                yield return new WaitForSeconds(0.1f);
                resultCharImages.Add(Instantiate(c.charImage, gridParent));
            }
        }

        isResult = true;
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
            StartCoroutine(blurCircleSizeUpCoroutine()); // ��� ��Ŭ�� ũ�� ����
            isGacha = false; // �� �̻� Ŭ���� �� ������ ����
        }
    }

    private void ChangeAcornColorByClickCount()
    {
        // ���� ĳ������ ��� ����
        CharGrade finalGrade = bestGrade;

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
        //rateCircle.SetActive(true);
    }

    private IEnumerator blurCircleSizeUpCoroutine()
    {
        if (getCharaterIndex >= getCharaterInfos.Count)
        {
            getCharaterIndex = 0;
            yield break;
        }

        rateCircle.SetActive(true);

        GachaCharacterInfo selectedCharacter = getCharaterInfos[getCharaterIndex];

        RectTransform rateCircleRect = rateCircle.GetComponent<RectTransform>();
        Image rateCircleImage = rateCircle.GetComponent<Image>();
        Image backgroundPanelImage = backgroundPanel.GetComponent<Image>();

        // ��Ŭ �ʱ� ũ�⸦ ���� (������ �� ���� ũ�⿡�� ����)
        rateCircleRect.localScale = Vector3.zero;
        rateCircleImage.color = new Color(rateCircleImage.color.r, rateCircleImage.color.g, rateCircleImage.color.b, 0);
        backgroundPanelImage.color = new Color(0, 0, 0, 0); // ��ο� �г� ����

        // ��Ŭ ���İ� �ִϸ��̼�
        DOTween.ToAlpha(() => rateCircleImage.color, color => rateCircleImage.color = color, 1f, 0.2f);

        // ��ο� ��� ���� �ִϸ��̼�
        DOTween.ToAlpha(() => backgroundPanelImage.color, color => backgroundPanelImage.color = color, 0.6f, 0.3f);

        // ��Ŭ ũ�� �ִϸ��̼�
        yield return rateCircleRect.DOScale(10f, 0.7f).SetEase(Ease.OutBack).WaitForCompletion();

        // Glow Intensity ���� 0���� ���̱� ���� �ִϸ��̼� �߰�
        Material rateCircleMaterial = rateCircle.GetComponent<Image>().material; // Material ��������
        float currentGlowIntensity = rateCircleMaterial.GetFloat("_GlowIntensity");

        // Glow Intensity�� 0���� �ִϸ��̼� ó��
        yield return DOTween.To(() => currentGlowIntensity, x => rateCircleMaterial.SetFloat("_GlowIntensity", x), 1f, 0.5f).WaitForCompletion();
        // Glow Intensity�� 0�� �� �� OnGradeCircleAnimationComplete ����
        OnGradeCircleAnimationComplete();
    }


    // �ִϸ��̼��� ���� �� ������ ����� �����ϴ� �Լ�
    private void OnGradeCircleAnimationComplete()
    {
        resultPanelObj.SetActive(true);
        acorn.SetActive(false);
        rateCircle.SetActive(false);
        StartCoroutine(ResultCharaterImageSpawn());
    }

    private void InitGacha()
    {
        // 1. �̱� ���� �ʱ�ȭ
        isGacha = false;
        isResult = false;

        // 2. Ŭ�� Ƚ�� �ʱ�ȭ
        clickCount = 0;

        // 3. ���� ĳ���� �ε��� �ʱ�ȭ
        getCharaterIndex = 0;

        // 4. ���丮 ��ġ �ʱ�ȭ
        acorn.transform.position = acornStartPos.transform.position;

        // 5. ���丮 ���� �ʱ�ȭ (�⺻ ����)
        acorn.GetComponentInChildren<Renderer>().material = acronDefultMaterial;

        // 6. ��� �г� �� ��� ��Ŭ �ʱ�ȭ
        backgroundPanel.SetActive(false); // ��ο� ��� ��Ȱ��ȭ
        rateCircle.SetActive(false);      // ��� ��Ŭ ��Ȱ��ȭ

        // 7. ��� �г� �ʱ�ȭ
        gachaPanel.SetActive(false);
        resultPanelObj.SetActive(false);  // ��� �г� ��Ȱ��ȭ

        // 8. ��� ĳ���� �̹��� �ʱ�ȭ
        foreach (var resultCharImage in resultCharImages)
        {
            Destroy(resultCharImage);  // ���� ��� ĳ���� �̹��� ����
        }
        resultCharImages.Clear();  // ����Ʈ�� �ʱ�ȭ

        // 10. ĳ���� ���� ����Ʈ �ʱ�ȭ (�ʿ� ��)
        getCharaterInfos.Clear();  // ���� ĳ���� ����Ʈ �ʱ�ȭ

        // 11. rateCircle Blur �� �ʱ�ȭ (�� ���� �⺻ ������ ����)
        Material rateCircleMaterial = rateCircle.GetComponent<Image>().material;

        // 12. rateCircle GlowIntensity �ʱ�ȭ
        if (rateCircleMaterial.HasProperty("_GlowIntensity"))
        {
            rateCircleMaterial.SetFloat("_GlowIntensity", 10);  // GlowIntensity�� �ʱ�ȭ
        }

        // 13. ���� �ְ� ��� �ʱ�ȭ
        bestGrade = CharGrade.Common;

        // 14. ���� �ʱ�ȭ
        acorn.transform.localEulerAngles = new Vector3(-63.866f, 0f, 0f); // �⺻ �����̼� ��
    }

}