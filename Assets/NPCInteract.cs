using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    public string npcName = "NPC";
    public DialogueData dialogueData;
    public float interactRadius = 3f;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive()) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactRadius;

        if (inRange && !playerInRange)
        {
            playerInRange = true;
            InteractionUI.Instance?.Show($"[E] Talk to {npcName}");
        }
        else if (!inRange && playerInRange)
        {
            playerInRange = false;
            InteractionUI.Instance?.Hide();
        }

        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E键被按下！dialogueData=" + (dialogueData != null));
            if (dialogueData != null)
            {
                Debug.Log("对话行数：" + dialogueData.lines.Length);
                DialogueManager.Instance?.StartDialogue(dialogueData);
            }
            else
            {
                Debug.LogWarning("dialogueData 是空的！");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
