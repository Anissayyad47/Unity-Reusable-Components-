using UnityEngine;

[CreateAssetMenu(menuName ="Dialogue/Actor")]
public class DialogueActor : ScriptableObject
{
    public string actorName;
    public Sprite portrait;
    public SpeakerType speakerType;
}
