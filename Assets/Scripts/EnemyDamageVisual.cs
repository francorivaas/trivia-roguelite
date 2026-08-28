using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageVisual : MonoBehaviour
{
    [System.Serializable]
    public class DamageMilestone
    {
        [Min(0)]
        public int damageTaken;

        public Sprite sprite;
    }

    [Header("References")]
    [SerializeField] private Image enemyImage;

    [Header("Sprites")]
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private DamageMilestone[] milestones;

    private void Awake()
    {
        if (enemyImage == null)
            enemyImage = GetComponent<Image>();

        if (baseSprite == null && enemyImage != null)
            baseSprite = enemyImage.sprite;
    }

    public void RefreshVisual(int currentHealth, int maxHealth)
    {
        if (enemyImage == null)
            return;

        int totalDamageTaken = maxHealth - currentHealth;

        Sprite selectedSprite = baseSprite;
        int highestReachedMilestone = -1;

        foreach (DamageMilestone milestone in milestones)
        {
            if (milestone.sprite == null)
                continue;

            if (totalDamageTaken >= milestone.damageTaken &&
                milestone.damageTaken >= highestReachedMilestone)
            {
                selectedSprite = milestone.sprite;
                highestReachedMilestone = milestone.damageTaken;
            }
        }

        if (selectedSprite != null)
            enemyImage.sprite = selectedSprite;
    }
}