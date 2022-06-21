using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        public string uniqueId;
        public string text;
        public List<string> children = new List<string>();
        public Rect rect = new Rect(0, 0, 300, 150);
    }
}
