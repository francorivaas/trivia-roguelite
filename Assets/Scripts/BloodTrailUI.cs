using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BloodTrailUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float startThicknessMultiplier = 0.6f;
    [SerializeField] private float endThicknessMultiplier = 1.15f;

    public void Initialize(
        Vector2 startPos,
        Vector2 endPos,
        float thickness,
        Color color,
        float customDuration = -1f)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (image == null)
            image = GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (customDuration > 0f)
            duration = customDuration;

        Vector2 direction = endPos - startPos;
        float length = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rectTransform.anchoredPosition = (startPos + endPos) * 0.5f;
        rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        rectTransform.sizeDelta = new Vector2(length, thickness);

        image.color = color;

        StartCoroutine(AnimateRoutine(length, thickness));
    }

    private IEnumerator AnimateRoutine(float length, float thickness)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float currentThickness = Mathf.Lerp(
                thickness * startThicknessMultiplier,
                thickness * endThicknessMultiplier,
                t
            );

            rectTransform.sizeDelta = new Vector2(length, currentThickness);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}