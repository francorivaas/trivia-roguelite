using UnityEngine;

public class DamageFeedbackUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private RectTransform feedbackRoot;
    [SerializeField] private Camera uiCamera;

    [Header("Prefabs")]
    [SerializeField] private BloodTrailUI bloodTrailPrefab;
    [SerializeField] private FloatingDamageTextUI floatingDamageTextPrefab;

    [Header("Enemy Hit Settings")]
    [SerializeField] private Color enemyDamageColor = new Color(0.85f, 0.05f, 0.05f);
    [SerializeField] private float enemyTrailThickness = 28f;
    [SerializeField] private float enemyTrailDuration = 0.22f;
    [SerializeField] private Vector2 enemyTextOffsetMin = new Vector2(-20f, 10f);
    [SerializeField] private Vector2 enemyTextOffsetMax = new Vector2(25f, 45f);

    [Header("Player Hit Settings")]
    [SerializeField] private Color playerDamageColor = new Color(1f, 0.45f, 0.15f);
    [SerializeField] private float playerTrailThickness = 24f;
    [SerializeField] private float playerTrailDuration = 0.22f;
    [SerializeField] private Vector2 playerTextOffsetMin = new Vector2(-20f, 10f);
    [SerializeField] private Vector2 playerTextOffsetMax = new Vector2(25f, 45f);

    public void ShowHit(int damage, RectTransform origin, RectTransform target, bool targetIsEnemy)
    {
        if (feedbackRoot == null || origin == null || target == null)
            return;

        Vector2 startLocal = GetAnchoredPositionInRoot(origin);
        Vector2 endLocal = GetAnchoredPositionInRoot(target);

        Color color = targetIsEnemy ? enemyDamageColor : playerDamageColor;
        float thickness = targetIsEnemy ? enemyTrailThickness : playerTrailThickness;
        float duration = targetIsEnemy ? enemyTrailDuration : playerTrailDuration;

        // 1) Trail
        if (bloodTrailPrefab != null)
        {
            BloodTrailUI trailInstance = Instantiate(bloodTrailPrefab, feedbackRoot);
            trailInstance.Initialize(startLocal, endLocal, thickness, color, duration);
        }

        // 2) Floating text
        if (floatingDamageTextPrefab != null)
        {
            Vector2 offsetMin = targetIsEnemy ? enemyTextOffsetMin : playerTextOffsetMin;
            Vector2 offsetMax = targetIsEnemy ? enemyTextOffsetMax : playerTextOffsetMax;

            Vector2 randomOffset = new Vector2(
                Random.Range(offsetMin.x, offsetMax.x),
                Random.Range(offsetMin.y, offsetMax.y)
            );

            FloatingDamageTextUI textInstance =
                Instantiate(floatingDamageTextPrefab, feedbackRoot);

            textInstance.Initialize(damage, endLocal + randomOffset, color, "-");
        }
    }

    private Vector2 GetAnchoredPositionInRoot(RectTransform target)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, target.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            feedbackRoot,
            screenPoint,
            uiCamera,
            out Vector2 localPoint
        );

        return localPoint;
    }
}