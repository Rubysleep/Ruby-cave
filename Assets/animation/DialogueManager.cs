using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continuePrompt;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public DialogueData prologueData;

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        dialoguePanel.SetActive(false);

        if (prologueData != null)
            StartDialogue(prologueData);
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue.lines[currentLineIndex].dialogueText;
                isTyping = false;
                continuePrompt.gameObject.SetActive(true);
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.lines.Length == 0) return;

        currentDialogue = data;
        currentLineIndex = 0;
        dialogueActive = true;
        dialoguePanel.SetActive(true);
        continuePrompt.gameObject.SetActive(false);

        InteractionUI.Instance?.Hide();
        SetPlayerControl(false);

        DisplayLine();
    }

    void DisplayLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        nameText.text = line.speakerName;
        continuePrompt.gameObject.SetActive(false);
        typingCoroutine = StartCoroutine(TypeText(line.dialogueText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        continuePrompt.gameObject.SetActive(true);
    }

    void NextLine()
    {
        currentLineIndex++;
        DisplayLine();
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
         if (currentDialogue != null && currentDialogue.loadSceneAfterDialogue 
            && !string.IsNullOrEmpty(currentDialogue.sceneToLoad))
        {
            SceneManager.LoadScene(currentDialogue.sceneToLoad);
        }
        
        currentDialogue = null;
        SetPlayerControl(true); 
    }

    void SetPlayerControl(bool enable)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        foreach (var script in player.GetComponents<MonoBehaviour>())
        {
            if (script.GetType().Name == "AndyController")
            {
                script.enabled = enable;
                break;
            }
        }
    }

    public bool IsDialogueActive() => dialogueActive;

}
