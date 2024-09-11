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
    [SerializeField] private GameObject backgroundPanel; // 어두운 배경 패널
    [SerializeField] private List<CharGradeInfo> charGradeInfos = new List<CharGradeInfo>();
    [SerializeField] private bool isGacha = false;
    [SerializeField] private int getCharaterIndex = 0;
    [SerializeField] private int clickCount = 0; // 클릭 횟수를 기록
    [SerializeField] private int maxClickCount = 3; // 등급 표시 전 최대 클릭 횟수
    [SerializeField] private Material acronDefultMaterial;
    [SerializeField] private Material acornBlurMaterial;

    [Header("Particles")]
    [SerializeField] private ParticleSystem clickParticlePrefab;  // 파티클 프리팹

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isGacha)
        {
            HandleClick();
        }
    }

    // 캐릭터를 랜덤하게 선택하는 함수
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

        Debug.LogError("Unexpected error: 캐릭터를 선택할 수 없습니다. 기본 캐릭터를 반환합니다.");
        return characterInfos[0];
    }

    // 한 번 뽑기 함수
    public void GachaOneTime()
    {
        getCharaterInfos.Add(RandomChar());
        acorn.GetComponentInChildren<Renderer>().material = acronDefultMaterial;
        Debug.Log("선택된 캐릭터: " + getCharaterInfos[0].charName);
        SetGradeCircle(getCharaterInfos[0].charGrade);
        AcornDive();
    }

    // 10번 뽑기 함수
    public void GachaTenTime()
    {
        for (int i = 0; i < 10; i++)
        {
            GachaOneTime();
        }
    }

    // 총 가중치 계산 및 초기화
    private void Start()
    {
        if (characterInfos == null || characterInfos.Count == 0)
        {
            Debug.LogError("캐릭터 정보 리스트가 비어있습니다! 뽑기를 실행할 수 없습니다.");
            return;
        }

        total = 0;
        foreach (var character in characterInfos)
        {
            if (character.weight <= 0)
            {
                Debug.LogError("가중치가 0 이하인 캐릭터가 있습니다: " + character.charName);
                continue;
            }

            total += character.weight;
        }

        if (total == 0)
        {
            Debug.LogError("모든 캐릭터의 가중치가 0입니다! 뽑기를 실행할 수 없습니다.");
        }
        else
        {
            Debug.Log("총 가중치: " + total);
        }
    }

    private void AcornDive()
    {
        gachaPanel.SetActive(true);

        // 도토리 위치 초기화
        acorn.transform.position = acornStartPos.transform.position;
        acorn.SetActive(true);

        StartCoroutine(AcornDiveAnim());
    }

    IEnumerator AcornDiveAnim()
    {
        Sequence sequence = DOTween.Sequence();

        // y값은 통통 튀는 애니메이션 (OutBounce 이징 사용) + y값 보정
        float yOffset = 0.1f;
        sequence.Append(acorn.transform
            .DOMoveY(acornLastPos.position.y - yOffset, 1f)
            .SetEase(Ease.OutBounce));

        // x값은 부드럽게 이동 (통통 튀지 않음)
        sequence.Join(acorn.transform
            .DOMoveZ(acornLastPos.position.z, 1f)
            .SetEase(Ease.Linear));

        yield return sequence.WaitForCompletion();

        isGacha = true;
    }

    private void HandleClick()
    {
        // 이미 최대 클릭 수에 도달했다면 더 이상 처리하지 않음
        if (clickCount >= maxClickCount)
        {
            return;
        }

        clickCount++;
        ShakeAcorn(); // 클릭할 때마다 도토리 흔들기
        SpawnClickParticle(); // 클릭 시 파티클 생성

        // 클릭 횟수에 따라 도토리 색상 변경
        ChangeAcornColorByClickCount();

        // 최대 클릭 수에 도달하면 등급 서클 표시 및 클릭 막음
        if (clickCount == maxClickCount)
        {
            GradeCircleSizeUp(); // 등급 서클을 크게 만듦
            isGacha = false; // 더 이상 클릭할 수 없도록 설정
        }
    }

    private void ChangeAcornColorByClickCount()
    {
        // 뽑힌 캐릭터의 등급 정보
        CharGrade finalGrade = getCharaterInfos[getCharaterIndex].charGrade;

        // 1번째 클릭에서는 무조건 커먼 색상
        if (clickCount == 1)
        {
            SetAcornColor(CharGrade.Common);
        }
        // 2번째 클릭에서는 레어 이상일 경우 레어 색상
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
        // 3번째 클릭에서는 뽑힌 캐릭터의 최종 등급 색상
        else if (clickCount == 3)
        {
            SetAcornColor(finalGrade);
            //isGacha = false;
        }
    }

    private void ShakeAcorn()
    {
        // 현재 로컬 회전 값을 저장합니다.
        Vector3 currentRotation = acorn.transform.localEulerAngles;

        // y축을 기준으로 흔들림 각도 설정 (좌우 흔들림)
        float shakeAngle = 15f; // 흔들릴 각도 (기울기)
        float shakeDuration = 0.3f; // 흔들릴 시간

        // 좌우로 흔들리도록 로컬 회전을 설정합니다.
        acorn.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y + shakeAngle, currentRotation.z), shakeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)  // 왕복
            .OnComplete(() =>
            {
                // 애니메이션이 끝나면 원래 각도로 되돌립니다.
                acorn.transform.localEulerAngles = currentRotation;
            });
    }

    private void SpawnClickParticle()
    {
        // 클릭한 위치를 스크린 좌표에서 월드 좌표로 변환
        Vector3 clickPosition = Input.mousePosition;
        clickPosition.z = 10f; // 카메라에서의 거리 설정 (카메라와 클릭 위치 간의 거리)
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);

        // 파티클을 클릭 위치에서 생성
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

        // 서클 초기 크기를 설정 (시작할 때 작은 크기에서 시작)
        rateCircleRect.localScale = Vector3.zero;
        rateCircleImage.color = new Color(rateCircleImage.color.r, rateCircleImage.color.g, rateCircleImage.color.b, 0);
        backgroundPanelImage.color = new Color(0, 0, 0, 0); // 어두운 패널 투명도

        rateCircleRect.DOScale(8f, 0.4f).SetEase(Ease.OutBack); // 서클 크기 애니메이션
        DOTween.ToAlpha(() => rateCircleImage.color, color => rateCircleImage.color = color, 1f, 0.2f); // 서클 알파값 애니메이션
        DOTween.ToAlpha(() => backgroundPanelImage.color, color => backgroundPanelImage.color = color, 0.6f, 0.3f); // 어두운 배경 투명도 애니메이션
    }
}