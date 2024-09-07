using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // UI 관련 기능을 사용하기 위해 추가

public class ButtonSelect : MonoBehaviour
{
    public Button myButton; // 버튼 컴포넌트를 가져올 변수
    public Animator animator; // 애니메이터 컴포넌트 변수
    private bool isSelected = false; // 버튼이 선택되었는지 여부를 저장할 변수

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    void Start()
    {
        myButton = GetComponent<Button>();
        // 버튼에 리스너를 추가하여 클릭될 때마다 선택 상태를 바꿉니다.
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // 현재 선택된 게임 오브젝트가 버튼인지 확인
        if (EventSystem.current.currentSelectedGameObject == myButton.gameObject)
        {
            // 버튼이 선택되어 있으면 'isSelect'를 true로 설정
            animator.SetBool("isSelect", true);
        }
        else
        {
            // 버튼이 선택되지 않으면 'isSelect'를 false로 설정
            animator.SetBool("isSelect", false);
        }
    }

    // 버튼이 클릭될 때 호출되는 함수
    void OnButtonClick()
    {
        // 버튼이 클릭되면 선택 상태를 변경
        isSelected = !isSelected;
    }
}
