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


    // 캐릭터를 랜덤하게 선택하는 함수
    public GachaCharacterInfo RandomChar()
    {
        float weight = 0;
        float selectNum = Random.Range(0f, total); // 1부터 total까지 랜덤 선택

        //Debug.Log("랜덤으로 선택된 숫자: " + selectNum); // 랜덤 선택 값 확인
        foreach (GachaCharacterInfo c in characterInfos)
        {
            weight += c.weight;
            //Debug.Log(c.charName + " 캐릭터의 누적 가중치: " + weight); // 캐릭터 가중치 확인

            // 여기서 선택된 캐릭터를 반환합니다.
            if (selectNum <= weight)
            {
                //Debug.Log("선택된 캐릭터: " + c.charName);
                return c;
            }
        }

        // 이 시점에선 캐릭터가 무조건 선택되어야 하므로 여기는 절대 도달하지 않습니다.
        // 만약 실행된다면, 이는 예상치 못한 버그가 있다는 의미입니다.
        Debug.LogError("Unexpected error: 캐릭터를 선택할 수 없습니다. 기본 캐릭터를 반환합니다.");
        return characterInfos[0]; // 기본 캐릭터 반환 (이 부분은 거의 실행되지 않을 것)
    }


    // 한 번 뽑기 함수
    public void GachaOneTime()
    {
        GachaCharacterInfo character = RandomChar();
        Debug.Log("선택된 캐릭터: " + character.charName); // 선택된 캐릭터 이름 출력
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
        // 캐릭터 정보가 없는 경우 방지
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
                continue; // 가중치가 0 이하인 캐릭터는 제외
            }

            total += character.weight;
        }

        // 총 가중치가 0인 경우 방지
        if (total == 0)
        {
            Debug.LogError("모든 캐릭터의 가중치가 0입니다! 뽑기를 실행할 수 없습니다.");
        }
        else
        {
            Debug.Log("총 가중치: " + total); // 총 가중치 확인
        }
    }


    IEnumerator PlayGachaAnime(CharGrade charGrade)
    {
        gachaPanel.SetActive(true);
        yield return null;

        // 도토리 위치 초기화
        Acorn.transform.position = acornStartPos.transform.position;
        Acorn.SetActive(true);

        // x값과 y값을 각각 제어하는 애니메이션
        Sequence sequence = DOTween.Sequence();  // DOTween의 Sequence 사용

        // y값은 통통 튀는 애니메이션 (OutBounce 이징 사용) + y값 보정
        float yOffset = 0.1f;  // y 좌표에 대한 보정값
        sequence.Append(Acorn.transform
            .DOMoveY(acornLastPos.position.y - yOffset, 1f)  // 목표 y 위치로 이동 (보정 적용)
            .SetEase(Ease.OutBounce));

        // x값은 부드럽게 이동 (통통 튀지 않음)
        sequence.Join(Acorn.transform
            .DOMoveZ(acornLastPos.position.z, 1f)  // 목표 x 위치로 1초 동안 이동
            .SetEase(Ease.Linear));

        yield return sequence.WaitForCompletion();  // 애니메이션 완료까지 대기

        foreach (var gradeInfo in charGradeInfos)
        {
            if (gradeInfo.gradeName == charGrade.ToString())
            {
                rateCircle.GetComponent<Image>().color = gradeInfo.color;
            }
        }
    }
}
