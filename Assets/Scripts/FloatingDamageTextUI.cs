using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageTextUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    [Header("Animation")]
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float moveUpDistance = 60f;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float endScale = 1.1f;

    public void Initialize(int damage, Vector2 anchoredPosition, Color color, string prefix = "-")
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (damageText == null)
            damageText = GetComponentInChildren<TMP_Text>();

        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.localScale = Vector3.one * startScale;

        damageText.text = prefix + damage.ToString();
        damageText.color = color;

        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveUpDistance;

        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            rectTransform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}