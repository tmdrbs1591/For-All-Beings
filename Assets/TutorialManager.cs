using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TextAnim textAnim;
    public GameObject messagePanel;
    private PlayerCtrl playerController; // PlayerController ��ũ��Ʈ�� ������ ������ ����

    // Start is called before the first frame update
    void Start()
    {
        CharManager.instance.currentCharacter = Character.Knight;
        StartCoroutine(TutorialStart());
    }

    // Update is called once per frame
    void Update()
    {
        // ������Ʈ ���� (�ʿ信 ���� �߰�)
    }

    IEnumerator TutorialStart()
    {
        // Player �±׸� ���� ���� ������Ʈ�� ã���ϴ�.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // ���� ������Ʈ�� null�� �ƴϸ� PlayerController ��ũ��Ʈ�� �����ɴϴ�.
        if (player != null)
        {
            playerController = player.GetComponent<PlayerCtrl>();

        }
        playerController.enabled = false;
        // ��� �ð�
        yield return new WaitForSeconds(3f);

         Text("�ȳ� �ݰ���!");
        yield return new WaitForSeconds(2f);
        Text("�⺻���� ���� ����� ������� !");
        yield return new WaitForSeconds(2f);
        Text("W,A,S,D �� ���� �̵��� �� �־�!");
        yield return new WaitForSeconds(2f);
        Text("�տ� ��ǥ �������� �̵��غ�!");
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
