using UnityEngine;
using DG.Tweening;

public class DangerLine : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 1f; // ������ �ִϸ��̼��� ���� �ð�
    [SerializeField] private float moveDuration = 1f; // �̵� �ִϸ��̼��� ���� �ð�
    [SerializeField] private float targetScaleY = 5f; // ��ǥ y ������ ��
    [SerializeField] private float moveDistanceZ = 10f; // �̵��� z �Ÿ�

    private void OnEnable()
    {
        AnimateDangerLine();
    }

    private void AnimateDangerLine()
    {
        // ���� �������� ��������
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.localPosition;

        // y �� �������� �þ�� ȿ���� �ֱ� ����, �߰� ���� �������� �������� �����մϴ�.
        Vector3 targetScale = new Vector3(originalScale.x, targetScaleY, originalScale.z);
        Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + moveDistanceZ);

        // ������ �ִϸ��̼�: y ���� ��ǥ ������ Ȯ��
        transform.DOScaleY(targetScaleY, scaleDuration)
                 .SetEase(Ease.InOutSine)
                 .OnUpdate(() =>
                 {
                     // y �������� �����ϸ鼭 z �� �̵��� ����
                     float scaleFactor = transform.localScale.y / originalScale.y;
                     transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + moveDistanceZ * scaleFactor);
                 })
                 .OnComplete(() => {
                     // ������ �ִϸ��̼� �Ϸ� �� z �� �̵�
                     transform.DOMoveZ(targetPosition.z, moveDuration)
                              .SetEase(Ease.InOutSine);
                 });
    }
}
