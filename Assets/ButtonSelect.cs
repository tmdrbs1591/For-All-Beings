using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // UI ���� ����� ����ϱ� ���� �߰�

public class ButtonSelect : MonoBehaviour
{
    public Button myButton; // ��ư ������Ʈ�� ������ ����
    public Animator animator; // �ִϸ����� ������Ʈ ����
    private bool isSelected = false; // ��ư�� ���õǾ����� ���θ� ������ ����

    // Start�� ������ ���۵� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        myButton = GetComponent<Button>();
        // ��ư�� �����ʸ� �߰��Ͽ� Ŭ���� ������ ���� ���¸� �ٲߴϴ�.
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // ���� ���õ� ���� ������Ʈ�� ��ư���� Ȯ��
        if (EventSystem.current.currentSelectedGameObject == myButton.gameObject)
        {
            // ��ư�� ���õǾ� ������ 'isSelect'�� true�� ����
            animator.SetBool("isSelect", true);
        }
        else
        {
            // ��ư�� ���õ��� ������ 'isSelect'�� false�� ����
            animator.SetBool("isSelect", false);
        }
    }

    // ��ư�� Ŭ���� �� ȣ��Ǵ� �Լ�
    void OnButtonClick()
    {
        // ��ư�� Ŭ���Ǹ� ���� ���¸� ����
        isSelected = !isSelected;
    }
}
