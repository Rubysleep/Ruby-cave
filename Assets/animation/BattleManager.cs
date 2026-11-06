using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("玩家属性")]
    public string playerName = "Andy";
    public int playerMaxHP = 100;
    private int playerCurrentHP;
    private bool playerIsDefending = false;

    [Header("敌人属性")]
    public string enemyName = "Fish";
    public int enemyMaxHP = 80;
    private int enemyCurrentHP;
    private bool enemyIsDefending = false;

    [Header("UI - 玩家")]
    public Slider playerHPBar;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;

    [Header("UI - 敌人")]
    public Slider enemyHPBar;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyHPText;

    [Header("UI - 按钮")]
    public Button attackButton;
    public Button defendButton;

    [Header("战斗设置")]
    public int baseDamage = 30;
    public float turnDelay = 1f;

    private bool isPlayerTurn = true;
    private bool battleEnded = false;

    void Start()
    {
        playerCurrentHP = playerMaxHP;
        enemyCurrentHP = enemyMaxHP;

        playerNameText.text = playerName;
        enemyNameText.text = enemyName;

        attackButton.onClick.AddListener(PlayerAttack);
        defendButton.onClick.AddListener(PlayerDefend);

        UpdateUI();
    }

    void UpdateUI()
    {
        playerHPBar.maxValue = playerMaxHP;
        playerHPBar.value = playerCurrentHP;
        playerHPText.text = $"{playerCurrentHP}/{playerMaxHP}";

        enemyHPBar.maxValue = enemyMaxHP;
        enemyHPBar.value = enemyCurrentHP;
        enemyHPText.text = $"{enemyCurrentHP}/{enemyMaxHP}";
    }

    public void PlayerAttack()
    {
        if (!isPlayerTurn || battleEnded) return;

        int damage = CalculateDamage(baseDamage, enemyIsDefending);
        enemyCurrentHP = Mathf.Max(0, enemyCurrentHP - damage);
        enemyIsDefending = false;

        UpdateUI();

        if (enemyCurrentHP <= 0)
        {
            EndBattle(true);
            return;
        }

        isPlayerTurn = false;
        SetButtonsInteractable(false);
        Invoke(nameof(EnemyTurn), turnDelay);
    }

    public void PlayerDefend()
    {
        if (!isPlayerTurn || battleEnded) return;

        playerIsDefending = true;

        isPlayerTurn = false;
        SetButtonsInteractable(false);
        Invoke(nameof(EnemyTurn), turnDelay);
    }

    void EnemyTurn()
    {
        if (battleEnded) return;

        int action = Random.Range(0, 2);

        if (action == 0)
        {
            int damage = CalculateDamage(baseDamage, playerIsDefending);
            playerCurrentHP = Mathf.Max(0, playerCurrentHP - damage);
            playerIsDefending = false;
        }
        else
        {
            enemyIsDefending = true;
        }

        UpdateUI();

        if (playerCurrentHP <= 0)
        {
            EndBattle(false);
            return;
        }

        isPlayerTurn = true;
        SetButtonsInteractable(true);
    }

    int CalculateDamage(int damage, bool targetIsDefending)
    {
        if (targetIsDefending)
        {
            return Mathf.Max(1, damage / 3);
        }
        return damage;
    }

    void SetButtonsInteractable(bool enable)
    {
        attackButton.interactable = enable;
        defendButton.interactable = enable;
    }

    void EndBattle(bool playerWon)
    {
        battleEnded = true;
        SetButtonsInteractable(false);

        if (playerWon)
        {
            Debug.Log("战斗胜利！");
        }
        else
        {
            Debug.Log("战斗失败！");
        }

        Invoke(nameof(ReturnToMainScene), 3f);
    }

    void ReturnToMainScene()
    {
        SceneManager.LoadScene("SampleScene 1");
    }
}

