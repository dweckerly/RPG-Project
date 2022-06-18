using System.Collections;
using System.Collections.Generic;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueId;
        public string text;
        public string[] children;
    }
}
