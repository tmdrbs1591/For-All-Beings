using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBoss : MonoBehaviour
{
    private Enemy enemyScript; // Enemy 스크립트를 참조하기 위한 변수
    private float timer = 0f; // 시간을 추적하기 위한 변수
    private bool isTracking = false; // 추적이 시작되었는지 여부를 나타내는 변수

    public GameObject effect;

    // Start는 첫 프레임 업데이트 전에 호출됩니다
    void Start()
    {
        enemyScript = GetComponent<Enemy>(); // Enemy 스크립트를 가져옵니다
        enemyScript.enabled = false; // 처음에는 Enemy 스크립트를 비활성화합니다

        StartCoroutine(EffectOn());
    }

    // Update는 매 프레임마다 호출됩니다
    void Update()
    {
        timer += Time.deltaTime; // 타이머를 증가시킵니다

        if (timer >= 10f && !isTracking) // 8초가 지나면 Enemy 스크립트를 활성화합니다
        {
            enemyScript.enabled = true;
            isTracking = true; // 추적이 시작되었음을 표시합니다
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
