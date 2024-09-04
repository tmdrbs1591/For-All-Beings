using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPun, IPunObservable
{
    public float speed; // �̵� �ӵ�
    public float attackCoolTime = 0.5f;
    public float attackPower = 1f; // ����� attackPower ���� ���� ������� �ʱ� ���� private �ʵ�� ����

    public float maxHp;
    public float curHp;

    public float skillCoolTime = 5f; // ��ų ��Ÿ�� ����

    public LevelUp uiLevelUp;

    [Header("����")]
    public int playerLevel = 1;
    public float currentXp; // ���� ����ġ
    public float xp = 100; // �Ѱ���ġ

    [SerializeField] Slider xpSlider;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text xpText;
    [SerializeField] GameObject hitPanel;

    [SerializeField] GameObject originalMesh;
    [SerializeField] GameObject stoneGraveMesh;
    [SerializeField] GameObject diePanel;

    [SerializeField] public Slider reSpawnBar;

    private float keyPressTime = 0f;
    private  float requiredHoldTime = 3f;

    private PhotonView photonView;

    [SerializeField] public bool isDie;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Player_XP();
    }

    // Update is called once per frame
    void Update()
    {
        xpSlider.value = Mathf.Lerp(xpSlider.value, currentXp / xp, Time.deltaTime * 40f);
        levelText.text = "LV." + playerLevel.ToString();
        xpText.text = currentXp + "/" + xp;

        photonView.RPC("Die", RpcTarget.AllBuffered);

       reSpawnBar.value = keyPressTime / requiredHoldTime;
    }

    [PunRPC]
    void Die()
    {
        if (curHp <= 0)
        {
            isDie = true;
            originalMesh.SetActive(false);
            stoneGraveMesh.SetActive(true);
            diePanel.SetActive(true);
        }
    }
    [PunRPC]
   public void ReSpawn()
    {
        if (curHp <= 0)
        {
            isDie = false;
            originalMesh.SetActive(true);
            stoneGraveMesh.SetActive(false);
            diePanel.SetActive(false);
            curHp = 10;
        }
    }

    public void Player_XP()
    {
        xp = playerLevel * 100;
    }

    public void LV_UP()
    {
        if (currentXp >= xp)
        {
            AudioManager.instance.PlaySound(transform.position, 7, Random.Range(1f, 1f), 0.4f);
            currentXp -= xp;
            playerLevel++;
            Player_XP();

            // Photon RPC ȣ��
            photonView.RPC("UpdatePlayerStats", RpcTarget.AllBuffered, playerLevel, currentXp, xp);
            uiLevelUp.Show();
        }
    }
    [PunRPC]
    public void IncreaseHealth(float amount)
    {
        curHp += amount;
    }
    [PunRPC]
    void UpdatePlayerStats(int level, float currentXp, float xp)
    {
        this.playerLevel = level;
        this.currentXp = currentXp;
        this.xp = xp;
        Update(); // UI ������Ʈ
    }

    // IPunObservable �������̽� ����
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ ����
            stream.SendNext(playerLevel);
            stream.SendNext(currentXp);
            stream.SendNext(xp);
        }
        else
        {
            // ������ ����
            playerLevel = (int)stream.ReceiveNext();
            currentXp = (float)stream.ReceiveNext();
            xp = (float)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!isDie) { 
        curHp -= damage;
        StartCoroutine(HitPanelCor());
        }
    }
    public IEnumerator HitPanelCor()
    {

        CameraShake.instance.Shake();
        hitPanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitPanel.SetActive(false);

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") )
        {
            var playerStats = collision.gameObject.GetComponent<PlayerStats>();

         
                if (Input.GetKey(KeyCode.C))
                {
                    playerStats.reSpawnBar.gameObject.SetActive(true);
                    keyPressTime += Time.deltaTime; // "C" Ű�� ���� ������ Ÿ�̸� ����\


                    // Ÿ�̸Ӱ� 3�� �̻� �Ǹ� PerformAction �޼��带 ȣ��
                    if (keyPressTime >= requiredHoldTime)
                    {
                        playerStats.photonView.RPC("ReSpawn", RpcTarget.All);
                        keyPressTime = 0f; // �׼� ���� �� Ÿ�̸� �ʱ�ȭ
                    }
                }
                else
                {
                    // "C" Ű�� ���̸� Ÿ�̸Ӹ� �ʱ�ȭ
                    keyPressTime = 0f;
                }
            
        }
    }
  
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerStats = collision.gameObject.GetComponent<PlayerStats>();
            playerStats.reSpawnBar.gameObject.SetActive(false);
            keyPressTime = 0f; // Ÿ�̸Ӹ� �ʱ�ȭ
        }
    }



}
