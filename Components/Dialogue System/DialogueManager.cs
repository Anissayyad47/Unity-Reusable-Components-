using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private PlayerControllerV1 playerControllerV1;

    private DialogueData currentDialogue;
    private int currentLineIndex;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void StartDialogue(Comp_DialogueData dialogue)
    {
        if (dialogue == null || dialogue.lines.Length == 0)
            return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        playerControllerV1.enabled=false;

        ShowCurrentLine();
    }


    public void NextLine()
    {
        if (currentDialogue == null)
            return;

        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }


    private void ShowCurrentLine()
    {
        Comp_DialogueLine line =
            currentDialogue.lines[currentLineIndex];

        dialogueUI.ShowDialogue(line);
    }


    private void EndDialogue()
    {
        dialogueUI.HideDialogue();

        currentDialogue = null;
        currentLineIndex = 0;
        playerControllerV1.enabled=true;
    }
}
