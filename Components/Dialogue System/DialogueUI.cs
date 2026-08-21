using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private Image actorPortrait;

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerName;


    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }


    public void ShowDialogue(Comp_DialogueLine line)
    {
        dialoguePanel.SetActive(true);

        dialogueText.text = line.text;
        speakerName.text = line.actor.actorName;

        if (line.actor.speakerType == SpeakerType.Player)
        {
            actorPortrait.sprite = line.actor.portrait;
        }
        else if (line.actor.speakerType == SpeakerType.NPC)
        {
            actorPortrait.sprite = line.actor.portrait;
        }
    }


    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
