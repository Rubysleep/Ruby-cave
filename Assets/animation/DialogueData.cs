using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 5)]
    public string dialogueText;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;

    [Header("Scene")]
    public bool loadSceneAfterDialogue = false;  
    public string sceneToLoad = "SampleScene";    
}
