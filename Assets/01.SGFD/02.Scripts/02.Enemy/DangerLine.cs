using UnityEngine;
using DG.Tweening;

public class DangerLine : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 1f; // 스케일 애니메이션의 지속 시간
    [SerializeField] private float moveDuration = 1f; // 이동 애니메이션의 지속 시간
    [SerializeField] private float targetScaleY = 5f; // 목표 y 스케일 값
    [SerializeField] private float moveDistanceZ = 10f; // 이동할 z 거리

    private void OnEnable()
    {
        AnimateDangerLine();
    }

    private void AnimateDangerLine()
    {
        // 현재 스케일을 가져오기
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.localPosition;

        // y 축 방향으로 늘어나는 효과를 주기 위해, 중간 축을 기준으로 스케일을 조정합니다.
        Vector3 targetScale = new Vector3(originalScale.x, targetScaleY, originalScale.z);
        Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + moveDistanceZ);

        // 스케일 애니메이션: y 값을 목표 값으로 확대
        transform.DOScaleY(targetScaleY, scaleDuration)
                 .SetEase(Ease.InOutSine)
                 .OnUpdate(() =>
                 {
                     // y 스케일을 변경하면서 z 축 이동을 적용
                     float scaleFactor = transform.localScale.y / originalScale.y;
                     transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + moveDistanceZ * scaleFactor);
                 })
                 .OnComplete(() => {
                     // 스케일 애니메이션 완료 후 z 값 이동
                     transform.DOMoveZ(targetPosition.z, moveDuration)
                              .SetEase(Ease.InOutSine);
                 });
    }
}
