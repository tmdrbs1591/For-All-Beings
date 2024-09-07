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
    public Button shopButton;   // Setting 버튼

    public Animator menuAnimator;      // Menu 패널의 Animator
    public Animator characterAnimator; // Character 패널의 Animator
    public Animator settingAnimator;   // Setting 패널의 Animator
    public Animator shopButtonAnimator;   // Setting 패널의 Animator

    void Awake()
    {
        instance = this;

        // 버튼 클릭 시 각각 다른 패널로 전환
        menuButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Menu));
        characterButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Character));
        settingButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Setting));
        shopButton.onClick.AddListener(() => PanelSetting(CurrentPanel.Shop));
    }
    private void Update()
    {
      switch(currentPanel)
        {
            case CurrentPanel.Menu:
                menuAnimator.SetBool("isSelect", true);
                characterAnimator.SetBool("isSelect", false);
                settingAnimator.SetBool("isSelect", false);
                shopButtonAnimator.SetBool("isSelect", false);


                break;
            case CurrentPanel.Character:
                characterAnimator.SetBool("isSelect", true);
                menuAnimator.SetBool("isSelect", false);
                settingAnimator.SetBool("isSelect", false);
                shopButtonAnimator.SetBool("isSelect", false);

                break;
            case CurrentPanel.Setting:
                settingAnimator.SetBool("isSelect", true);
                menuAnimator.SetBool("isSelect", false);
                characterAnimator.SetBool("isSelect", false);
                shopButtonAnimator.SetBool("isSelect", false);

                break;
            case CurrentPanel.Shop:
                shopButtonAnimator.SetBool("isSelect", true);
                settingAnimator.SetBool("isSelect", false);
                menuAnimator.SetBool("isSelect", false);
                characterAnimator.SetBool("isSelect", false);

                break;
        }
    }

    public void PanelSetting(CurrentPanel panel)
    {
        currentPanel = panel;
        Debug.Log("Current Panel: " + currentPanel.ToString());
        // 여기에 각 패널에 맞는 로직 추가
    }
}
