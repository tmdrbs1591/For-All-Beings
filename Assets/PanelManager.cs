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
    public Button shopButton;   // Setting ��ư

    public Animator menuAnimator;      // Menu �г��� Animator
    public Animator characterAnimator; // Character �г��� Animator
    public Animator settingAnimator;   // Setting �г��� Animator
    public Animator shopButtonAnimator;   // Setting �г��� Animator

    void Awake()
    {
        instance = this;

        // ��ư Ŭ�� �� ���� �ٸ� �гη� ��ȯ
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
        // ���⿡ �� �гο� �´� ���� �߰�
    }
}
