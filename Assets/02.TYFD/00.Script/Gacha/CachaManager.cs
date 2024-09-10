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
    [SerializeField] private GameObject backgroundPanel; // 어두운 배경 패널
    [SerializeField] private List<CharGradeInfo> charGradeInfos = new List<CharGradeInfo>();
    [SerializeField] private bool isGacha = false;
    [SerializeField] private int getCharaterIndex = 0;
    [SerializeField] private int clickCount = 0; // 클릭 횟수를 기록
    [SerializeField] private int maxClickCount = 3; // 등급 표시 전 최대 클릭 횟수

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
        Debug.Log("선택된 캐릭터: " + getCharaterInfos[0].charName);
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
        Acorn.transform.position = acornStartPos.transform.position;
        Acorn.SetActive(true);

        StartCoroutine(AcornDiveAnim());
    }

    IEnumerator AcornDiveAnim()
    {
        Sequence sequence = DOTween.Sequence();

        // y값은 통통 튀는 애니메이션 (OutBounce 이징 사용) + y값 보정
        float yOffset = 0.1f;
        sequence.Append(Acorn.transform
            .DOMoveY(acornLastPos.position.y - yOffset, 1f)
            .SetEase(Ease.OutBounce));

        // x값은 부드럽게 이동 (통통 튀지 않음)
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
            ShakeAcorn(); // 클릭할 때마다 도토리 흔들기
            SpawnClickParticle(); // 클릭 시 파티클 생성
        }
        else
        {
            GradeCircleSizeUp(); // 최대 클릭 수 도달 후 등급 서클 표시
            clickCount = 0; // 클릭 카운트 초기화
        }
    }

    private void ShakeAcorn()
    {
        // 현재 로컬 회전 값을 저장합니다.
        Vector3 currentRotation = Acorn.transform.localEulerAngles;

        // y축을 기준으로 흔들림 각도 설정 (좌우 흔들림)
        float shakeAngle = 15f; // 흔들릴 각도 (기울기)
        float shakeDuration = 0.3f; // 흔들릴 시간

        // 좌우로 흔들리도록 로컬 회전을 설정합니다.
        Acorn.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y + shakeAngle, currentRotation.z), shakeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)  // 왕복
            .OnComplete(() =>
            {
                // 애니메이션이 끝나면 원래 각도로 되돌립니다.
                Acorn.transform.localEulerAngles = currentRotation;
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
        backgroundPanelImage.color = new Color(0, 0, 0, 0);  // 투명한 검정색

        // 배경을 어둡게 (1초 동안)
        backgroundPanel.SetActive(true);
        backgroundPanelImage.DOFade(0.5f, 1f);  // 반투명한 검정색으로 변화

        // 서클을 점차적으로 화면을 덮을 만큼 크게 확대 (2초 동안) + 서서히 투명도 증가
        rateCircleRect.DOScale(new Vector3(10f, 10f, 10f), 2f).SetEase(Ease.OutCubic);
        rateCircleImage.DOFade(1f, 1f);
    }
}
