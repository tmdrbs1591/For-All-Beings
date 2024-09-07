using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentPanel
{
    Menu,
    Character,
    Shop,
    Setting
}

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;

    public CurrentPanel currentPanel;

    public Button menuButton;      // Menu ��ư
    public Button characterButton; // Character ��ư
    public Button settingButton;   // Setting ��ư
    public Button shopButton;      // Shop ��ư

    public Animator menuAnimator;      // Menu �г��� Animator
    public Animator characterAnimator; // Character �г��� Animator
    public Animator settingAnimator;   // Setting �г��� Animator
    public Animator shopButtonAnimator;   // Shop �г��� Animator

    private Dictionary<CurrentPanel, Animator> panelAnimators;  // �гΰ� �ִϸ����� ����

    void Awake()
    {
        instance = this;

        // �гΰ� �ִϸ����͸� Dictionary�� ���
        panelAnimators = new Dictionary<CurrentPanel, Animator>()
        {
            { CurrentPanel.Menu, menuAnimator },
            { CurrentPanel.Character, characterAnimator },
            { CurrentPanel.Setting, settingAnimator },
            { CurrentPanel.Shop, shopButtonAnimator }
        };

        // ��ư Ŭ�� �� ���� �ٸ� �гη� ��ȯ
        menuButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Menu));
        characterButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Character));
        settingButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Setting));
        shopButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Shop));
    }

    private void Update()
    {
        UpdateAnimatorStates();
    }

    // ���� �гο� �´� �ִϸ����� ���¸� Ȱ��ȭ�ϴ� �Լ�
    private void UpdateAnimatorStates()
    {
        foreach (var entry in panelAnimators)
        {
            // ���� �гο� �ش��ϴ� �ִϸ����͸� 'isSelect'�� true�� ����, �������� false
            entry.Value.SetBool("isSelect", entry.Key == currentPanel);
        }
    }

    public void PanelSetting(CurrentPanel panel)
    {
        currentPanel = panel;
        Debug.Log("Current Panel: " + currentPanel.ToString());
    }
}
