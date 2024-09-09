using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TextAnim textAnim;
    public GameObject messagePanel;
    private PlayerCtrl playerController; // PlayerController 스크립트의 참조를 저장할 변수

    // Start is called before the first frame update
    void Start()
    {
        CharManager.instance.currentCharacter = Character.Knight;
        StartCoroutine(TutorialStart());
    }

    // Update is called once per frame
    void Update()
    {
        // 업데이트 로직 (필요에 따라 추가)
    }

    IEnumerator TutorialStart()
    {
        // Player 태그를 가진 게임 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // 게임 오브젝트가 null이 아니면 PlayerController 스크립트를 가져옵니다.
        if (player != null)
        {
            playerController = player.GetComponent<PlayerCtrl>();

        }
        playerController.enabled = false;
        // 대기 시간
        yield return new WaitForSeconds(3f);

         Text("안녕 반가워!");
        yield return new WaitForSeconds(2f);
        Text("기본적인 게임 방식을 배워보자 !");
        yield return new WaitForSeconds(2f);
        Text("W,A,S,D 를 눌러 이동할 수 있어!");
        yield return new WaitForSeconds(2f);
        Text("앞에 목표 지점까지 이동해봐!");
        yield return new WaitForSeconds(2f);
        playerController.enabled = true;

    }

    void Text(string text)
    {

        textAnim.textToShow = text;
        messagePanel.SetActive(false);
        messagePanel.SetActive(true);


    }
}
