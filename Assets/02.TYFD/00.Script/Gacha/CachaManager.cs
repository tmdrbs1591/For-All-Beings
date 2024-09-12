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
    [SerializeField] private bool isResult = false;
    [SerializeField] private int getCharaterIndex = 0;
    [SerializeField] private int clickCount = 0; // 클릭 횟수를 기록
    [SerializeField] private int maxClickCount = 3; // 등급 표시 전 최대 클릭 횟수
    [SerializeField] private Material acronDefultMaterial;
    [SerializeField] private Material acornBlurMaterial;

    [Header("Particles")]
    [SerializeField] private ParticleSystem clickParticlePrefab;  // 파티클 프리팹

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

    public void PlayGacha()
    {
        getCharaterInfos.Add(RandomChar());
        acorn.GetComponentInChildren<Renderer>().material = acronDefultMaterial;
    }

    // 한 번 뽑기 함수
    public void GachaOneTime()
    {
        AcornDive();
        PlayGacha();
    }

    // 10번 뽑기 함수
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
            // 1회 뽑기일 때 그리드 레이아웃 비활성화
            gridParent.GetComponent<GridLayoutGroup>().enabled = false;

            // 캐릭터 이미지를 중앙에 배치 (수동 배치)
            GameObject charImage = Instantiate(getCharaterInfos[0].charImage, gridParent);
            RectTransform charRect = charImage.GetComponent<RectTransform>();

            // 중앙 배치를 위해 앵커와 위치 조정
            charRect.anchorMin = new Vector2(0.5f, 0.5f);
            charRect.anchorMax = new Vector2(0.5f, 0.5f);
            charRect.pivot = new Vector2(0.5f, 0.5f);
            charRect.anchoredPosition = Vector2.zero; // 중앙에 배치
            resultCharImages.Add(charImage);
        }
        else if (getCharaterInfos.Count > 1)
        {
            // 10회 뽑기일 때 그리드 레이아웃 활성화
            gridParent.GetComponent<GridLayoutGroup>().enabled = true;

            // 캐릭터 이미지를 차례대로 그리드에 스폰
            foreach (GachaCharacterInfo c in getCharaterInfos)
            {
                yield return new WaitForSeconds(0.1f);
                resultCharImages.Add(Instantiate(c.charImage, gridParent));
            }
        }

        isResult = true;
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
            StartCoroutine(blurCircleSizeUpCoroutine()); // 등급 서클을 크게 만듦
            isGacha = false; // 더 이상 클릭할 수 없도록 설정
        }
    }

    private void ChangeAcornColorByClickCount()
    {
        // 뽑힌 캐릭터의 등급 정보
        CharGrade finalGrade = bestGrade;

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

        // 서클 초기 크기를 설정 (시작할 때 작은 크기에서 시작)
        rateCircleRect.localScale = Vector3.zero;
        rateCircleImage.color = new Color(rateCircleImage.color.r, rateCircleImage.color.g, rateCircleImage.color.b, 0);
        backgroundPanelImage.color = new Color(0, 0, 0, 0); // 어두운 패널 투명도

        // 서클 알파값 애니메이션
        DOTween.ToAlpha(() => rateCircleImage.color, color => rateCircleImage.color = color, 1f, 0.2f);

        // 어두운 배경 투명도 애니메이션
        DOTween.ToAlpha(() => backgroundPanelImage.color, color => backgroundPanelImage.color = color, 0.6f, 0.3f);

        // 서클 크기 애니메이션
        yield return rateCircleRect.DOScale(10f, 0.7f).SetEase(Ease.OutBack).WaitForCompletion();

        // Glow Intensity 값을 0으로 줄이기 위한 애니메이션 추가
        Material rateCircleMaterial = rateCircle.GetComponent<Image>().material; // Material 가져오기
        float currentGlowIntensity = rateCircleMaterial.GetFloat("_GlowIntensity");

        // Glow Intensity를 0으로 애니메이션 처리
        yield return DOTween.To(() => currentGlowIntensity, x => rateCircleMaterial.SetFloat("_GlowIntensity", x), 1f, 0.5f).WaitForCompletion();
        // Glow Intensity가 0이 된 후 OnGradeCircleAnimationComplete 실행
        OnGradeCircleAnimationComplete();
    }


    // 애니메이션이 끝난 후 실행할 기능을 정의하는 함수
    private void OnGradeCircleAnimationComplete()
    {
        resultPanelObj.SetActive(true);
        acorn.SetActive(false);
        rateCircle.SetActive(false);
        StartCoroutine(ResultCharaterImageSpawn());
    }

    private void InitGacha()
    {
        // 1. 뽑기 상태 초기화
        isGacha = false;
        isResult = false;

        // 2. 클릭 횟수 초기화
        clickCount = 0;

        // 3. 뽑힌 캐릭터 인덱스 초기화
        getCharaterIndex = 0;

        // 4. 도토리 위치 초기화
        acorn.transform.position = acornStartPos.transform.position;

        // 5. 도토리 색상 초기화 (기본 상태)
        acorn.GetComponentInChildren<Renderer>().material = acronDefultMaterial;

        // 6. 배경 패널 및 등급 서클 초기화
        backgroundPanel.SetActive(false); // 어두운 배경 비활성화
        rateCircle.SetActive(false);      // 등급 서클 비활성화

        // 7. 결과 패널 초기화
        gachaPanel.SetActive(false);
        resultPanelObj.SetActive(false);  // 결과 패널 비활성화

        // 8. 결과 캐릭터 이미지 초기화
        foreach (var resultCharImage in resultCharImages)
        {
            Destroy(resultCharImage);  // 이전 결과 캐릭터 이미지 제거
        }
        resultCharImages.Clear();  // 리스트도 초기화

        // 10. 캐릭터 정보 리스트 초기화 (필요 시)
        getCharaterInfos.Clear();  // 뽑힌 캐릭터 리스트 초기화

        // 11. rateCircle Blur 값 초기화 (블러 값을 기본 값으로 설정)
        Material rateCircleMaterial = rateCircle.GetComponent<Image>().material;

        // 12. rateCircle GlowIntensity 초기화
        if (rateCircleMaterial.HasProperty("_GlowIntensity"))
        {
            rateCircleMaterial.SetFloat("_GlowIntensity", 10);  // GlowIntensity를 초기화
        }

        // 13. 뽑은 최고 등급 초기화
        bestGrade = CharGrade.Common;

        // 14. 방향 초기화
        acorn.transform.localEulerAngles = new Vector3(-63.866f, 0f, 0f); // 기본 로테이션 값
    }

}