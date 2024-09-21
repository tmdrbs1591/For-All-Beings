using ExitGames.Demos.DemoPunVoice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] Image fadeIn;
    [SerializeField] Image fadeOut;

    [SerializeField] GameObject MenuPanel;

    [SerializeField] bool isMenulPanel = false;
    // Start is called before the first frame update
    void Start()
    {
        fadeOut.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isMenulPanel)
        { 
            MenuPanel.SetActive(true);
            isMenulPanel = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isMenulPanel)
        {
            MenuPanel.SetActive(false);
            isMenulPanel = false;
        }


    }

    public void LoadScene(string sceneName)
    {
       StartCoroutine( FadeScene(sceneName));
    }
    IEnumerator FadeScene(string sceneName)
    {
        fadeIn.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(sceneName);
    }

    public void CharChangeKnight()
    {
        CharManager.instance.currentCharacter = Character.Knight;

        Debug.Log("기사 선택");
    }
    public void CharChangeArcher()
    {
        CharManager.instance.currentCharacter = Character.Archer;
        Debug.Log("궁수 선택");

    }
    public void CharChangeDragoon()
    {
        CharManager.instance.currentCharacter = Character.Dragoon;
        Debug.Log("용기사 선택");

    }
    public void CharChangeMage()
    {
        CharManager.instance.currentCharacter = Character.Mage;
        Debug.Log("마법사 선택");

    }

    public void GameExit()
    {
        Application.Quit();
    }
}
