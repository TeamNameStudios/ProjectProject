using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Lines", menuName = "Create Dialogue Lines")]
public class TutorialDialogueScriptable : ScriptableObject
{
    public string dialogueName;
    public List<string> lines;
}
