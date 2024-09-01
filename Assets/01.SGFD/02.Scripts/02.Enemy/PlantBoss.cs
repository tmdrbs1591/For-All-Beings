using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBoss : MonoBehaviour
{
    private Enemy enemyScript; // Enemy ��ũ��Ʈ�� �����ϱ� ���� ����
    private float timer = 0f; // �ð��� �����ϱ� ���� ����
    private bool isTracking = false; // ������ ���۵Ǿ����� ���θ� ��Ÿ���� ����

    public GameObject effect;

    // Start�� ù ������ ������Ʈ ���� ȣ��˴ϴ�
    void Start()
    {
        enemyScript = GetComponent<Enemy>(); // Enemy ��ũ��Ʈ�� �����ɴϴ�
        enemyScript.enabled = false; // ó������ Enemy ��ũ��Ʈ�� ��Ȱ��ȭ�մϴ�

        StartCoroutine(EffectOn());
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�
    void Update()
    {
        timer += Time.deltaTime; // Ÿ�̸Ӹ� ������ŵ�ϴ�

        if (timer >= 10f && !isTracking) // 8�ʰ� ������ Enemy ��ũ��Ʈ�� Ȱ��ȭ�մϴ�
        {
            enemyScript.enabled = true;
            isTracking = true; // ������ ���۵Ǿ����� ǥ���մϴ�
        }
    }
    IEnumerator EffectOn()
    {
        yield return new WaitForSeconds(2f);
        effect.SetActive(true);
    }

    private void OnDestroy()
    {
        SongManager.instance.InGameSongPlay();
    }
}
