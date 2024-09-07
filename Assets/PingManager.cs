using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PingManager : MonoBehaviourPun
{
    [SerializeField] private GameObject questionMarkPing;  // ����ǥ �� ������
    [SerializeField] private GameObject locationMarkPing;  // ��ġ ǥ�� �� ������
    [SerializeField] private Camera mainCamera;            // ����� ī�޶�

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // Ű���� ���� 1 ��ư�� �����Ͽ� questionMarkPing ��ȯ
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnPing(questionMarkPing);
            AudioManager.instance.PlaySound(transform.position, 15, Random.Range(1f, 1f), 0.4f);

        }

        // Ű���� ���� 2 ��ư�� �����Ͽ� locationMarkPing ��ȯ
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnPing(locationMarkPing);
            AudioManager.instance.PlaySound(transform.position, 16, Random.Range(1f, 1f), 0.4f);

        }
    }

    // ���콺 Ŭ�� ������ �� ��ȯ
    void SpawnPing(GameObject pingPrefab)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ����ĳ��Ʈ�� �浹�� ������ �� ������Ʈ�� ��Ʈ��ũ �� ��ȯ
        if (Physics.Raycast(ray, out hit))
        {
            GameObject pingObject = PhotonNetwork.Instantiate(pingPrefab.name, hit.point, Quaternion.identity);

            // 4�� �Ŀ� �� ������Ʈ ����
            StartCoroutine(DestroyPingAfterDelay(pingObject, 4f));
        }
    }

    // 4�� �ڿ� ���� �����ϴ� �ڷ�ƾ
    IEnumerator DestroyPingAfterDelay(GameObject pingObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ��Ʈ��ũ �󿡼� ������Ʈ ����
        if (pingObject != null && pingObject.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(pingObject);
        }
    }
}
