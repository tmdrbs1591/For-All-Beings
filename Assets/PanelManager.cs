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

    public Button menuButton;      // Menu 버튼
    public Button characterButton; // Character 버튼
    public Button settingButton;   // Setting 버튼
    public Button shopButton;      // Shop 버튼

    public Animator menuAnimator;      // Menu 패널의 Animator
    public Animator characterAnimator; // Character 패널의 Animator
    public Animator settingAnimator;   // Setting 패널의 Animator
    public Animator shopButtonAnimator;   // Shop 패널의 Animator

    private Dictionary<CurrentPanel, Animator> panelAnimators;  // 패널과 애니메이터 매핑

    void Awake()
    {
        instance = this;

        // 패널과 애니메이터를 Dictionary에 등록
        panelAnimators = new Dictionary<CurrentPanel, Animator>()
        {
            { CurrentPanel.Menu, menuAnimator },
            { CurrentPanel.Character, characterAnimator },
            { CurrentPanel.Setting, settingAnimator },
            { CurrentPanel.Shop, shopButtonAnimator }
        };

        // 버튼 클릭 시 각각 다른 패널로 전환
        menuButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Menu));
        characterButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Character));
        settingButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Setting));
        shopButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Shop));
    }

    private void Update()
    {
        UpdateAnimatorStates();
    }

    // 현재 패널에 맞는 애니메이터 상태만 활성화하는 함수
    private void UpdateAnimatorStates()
    {
        foreach (var entry in panelAnimators)
        {
            // 현재 패널에 해당하는 애니메이터만 'isSelect'를 true로 설정, 나머지는 false
            entry.Value.SetBool("isSelect", entry.Key == currentPanel);
        }
    }

    public void PanelSetting(CurrentPanel panel)
    {
        currentPanel = panel;
        Debug.Log("Current Panel: " + currentPanel.ToString());
    }
}
