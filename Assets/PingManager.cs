using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PingManager : MonoBehaviourPun
{
    [SerializeField] private GameObject questionMarkPing;  // 물음표 핑 프리팹
    [SerializeField] private GameObject locationMarkPing;  // 위치 표시 핑 프리팹
    [SerializeField] private Camera mainCamera;            // 사용할 카메라

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // 키보드 숫자 1 버튼을 감지하여 questionMarkPing 소환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnPing(questionMarkPing);
            AudioManager.instance.PlaySound(transform.position, 15, Random.Range(1f, 1f), 0.4f);

        }

        // 키보드 숫자 2 버튼을 감지하여 locationMarkPing 소환
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnPing(locationMarkPing);
            AudioManager.instance.PlaySound(transform.position, 16, Random.Range(1f, 1f), 0.4f);

        }
    }

    // 마우스 클릭 지점에 핑 소환
    void SpawnPing(GameObject pingPrefab)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 레이캐스트가 충돌한 지점에 핑 오브젝트를 네트워크 상에 소환
        if (Physics.Raycast(ray, out hit))
        {
            GameObject pingObject = PhotonNetwork.Instantiate(pingPrefab.name, hit.point, Quaternion.identity);

            // 4초 후에 핑 오브젝트 제거
            StartCoroutine(DestroyPingAfterDelay(pingObject, 4f));
        }
    }

    // 4초 뒤에 핑을 제거하는 코루틴
    IEnumerator DestroyPingAfterDelay(GameObject pingObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 네트워크 상에서 오브젝트 제거
        if (pingObject != null && pingObject.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(pingObject);
        }
    }
}
