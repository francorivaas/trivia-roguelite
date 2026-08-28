using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriviaBattleManager : MonoBehaviour
{
    [Header("QUESTIONS")]
    [SerializeField] private List<TriviaQuestion> questions;

    [Header("UI - TRIVIA")]
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerButtonTexts;

    [SerializeField] private TMP_Text feedbackText;

    [Header("PLAYER")]
    [SerializeField] private int playerMaxHealth = 100;

    [SerializeField] private int playerMinDamageReceived = 10;
    [SerializeField] private int playerMaxDamageReceived = 20;

    [SerializeField] private Slider playerHealthSlider;

    [Header("ENEMY")]
    [SerializeField] private int enemyMaxHealth = 100;

    [SerializeField] private int enemyMinDamageReceived = 10;
    [SerializeField] private int enemyMaxDamageReceived = 20;

    [SerializeField] private Slider enemyHealthSlider;

    [SerializeField] private EnemyDamageVisual enemyDamageVisual;

    [Header("GAME")]
    [SerializeField] private float timeBetweenQuestions = 1f;

    private int playerCurrentHealth;
    private int enemyCurrentHealth;

    private TriviaQuestion currentQuestion;

    private List<int> questionBag = new List<int>();

    private bool waitingForNextQuestion;
    private bool gameEnded;

    [Header("DAMAGE FEEDBACK")]
    [SerializeField] private DamageFeedbackUI damageFeedbackUI;

    [SerializeField] private RectTransform playerAttackOrigin;
    [SerializeField] private RectTransform enemyHitPoint;

    [SerializeField] private RectTransform enemyAttackOrigin;
    [SerializeField] private RectTransform playerHitPoint;
    private void Start()
    {
        InitializeButtons();
        InitializeBattle();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int answerIndex = i;

            answerButtons[i].onClick.AddListener(
                () => AnswerQuestion(answerIndex)
            );
        }
    }

    private void InitializeBattle()
    {
        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;

        playerHealthSlider.minValue = 0;
        playerHealthSlider.maxValue = playerMaxHealth;
        playerHealthSlider.value = playerCurrentHealth;

        enemyHealthSlider.minValue = 0;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyCurrentHealth;

        if (feedbackText != null)
            feedbackText.text = "";

        if (enemyDamageVisual != null)
        {
            enemyDamageVisual.RefreshVisual(
                enemyCurrentHealth,
                enemyMaxHealth
            );
        }

        FillQuestionBag();

        ShowNextQuestion();
    }

    private void FillQuestionBag()
    {
        questionBag.Clear();

        for (int i = 0; i < questions.Count; i++)
        {
            questionBag.Add(i);
        }

        ShuffleQuestionBag();
    }

    private void ShuffleQuestionBag()
    {
        for (int i = 0; i < questionBag.Count; i++)
        {
            int randomIndex = Random.Range(i, questionBag.Count);

            int temp = questionBag[i];
            questionBag[i] = questionBag[randomIndex];
            questionBag[randomIndex] = temp;
        }
    }

    private void ShowNextQuestion()
    {
        if (gameEnded)
            return;

        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No hay preguntas cargadas.");
            return;
        }

        // Si ya usamos todas, volvemos a mezclarlas.
        if (questionBag.Count == 0)
        {
            FillQuestionBag();
        }

        int questionIndex = questionBag[0];
        questionBag.RemoveAt(0);

        currentQuestion = questions[questionIndex];

        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtonTexts.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                answerButtonTexts[i].text =
                    currentQuestion.answers[i];
            }
        }

        SetAnswerButtonsInteractable(true);

        waitingForNextQuestion = false;
    }

    public void AnswerQuestion(int answerIndex)
    {
        if (waitingForNextQuestion || gameEnded)
            return;

        waitingForNextQuestion = true;

        SetAnswerButtonsInteractable(false);

        bool isCorrect =
            answerIndex == currentQuestion.correctAnswerIndex;

        if (isCorrect)
        {
            DamageEnemy();
        }
        else
        {
            DamagePlayer();
        }

        if (!gameEnded)
        {
            StartCoroutine(NextQuestionRoutine());
        }
    }

    private void DamageEnemy()
    {
        int minDamage = Mathf.Min(enemyMinDamageReceived, enemyMaxDamageReceived);
        int maxDamage = Mathf.Max(enemyMinDamageReceived, enemyMaxDamageReceived);

        int damage = Random.Range(minDamage, maxDamage + 1);

        enemyCurrentHealth -= damage;
        enemyCurrentHealth = Mathf.Max(enemyCurrentHealth, 0);

        enemyHealthSlider.value = enemyCurrentHealth;

        if (feedbackText != null)
        {
            feedbackText.text = "¡Correcto! Hiciste " + damage + " de daño.";
        }

        if (damageFeedbackUI != null)
        {
            damageFeedbackUI.ShowHit(
                damage,
                playerAttackOrigin,
                enemyHitPoint,
                true
            );
        }

        if (enemyDamageVisual != null)
        {
            enemyDamageVisual.RefreshVisual(enemyCurrentHealth, enemyMaxHealth);
        }

        if (enemyCurrentHealth <= 0)
        {
            WinGame();
        }
    }

    private void DamagePlayer()
    {
        int minDamage = Mathf.Min(playerMinDamageReceived, playerMaxDamageReceived);
        int maxDamage = Mathf.Max(playerMinDamageReceived, playerMaxDamageReceived);

        int damage = Random.Range(minDamage, maxDamage + 1);

        playerCurrentHealth -= damage;
        playerCurrentHealth = Mathf.Max(playerCurrentHealth, 0);

        playerHealthSlider.value = playerCurrentHealth;

        if (feedbackText != null)
        {
            feedbackText.text = "Incorrecto. Recibiste " + damage + " de daño.";
        }

        if (damageFeedbackUI != null)
        {
            damageFeedbackUI.ShowHit(
                damage,
                enemyAttackOrigin,
                playerHitPoint,
                false
            );
        }

        if (playerCurrentHealth <= 0)
        {
            LoseGame();
        }
    }

    private IEnumerator NextQuestionRoutine()
    {
        yield return new WaitForSeconds(
            timeBetweenQuestions
        );

        if (feedbackText != null)
            feedbackText.text = "";

        ShowNextQuestion();
    }

    private void SetAnswerButtonsInteractable(bool value)
    {
        foreach (Button button in answerButtons)
        {
            button.interactable = value;
        }
    }

    private void WinGame()
    {
        gameEnded = true;

        SetAnswerButtonsInteractable(false);

        questionText.text = "¡ENEMIGO DERROTADO!";

        if (feedbackText != null)
            feedbackText.text = "¡Victoria!";
    }

    private void LoseGame()
    {
        gameEnded = true;

        SetAnswerButtonsInteractable(false);

        questionText.text = "HAS SIDO DERROTADO";

        if (feedbackText != null)
            feedbackText.text = "Derrota";
    }
}