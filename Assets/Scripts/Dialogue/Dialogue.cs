using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR        
        private void Awake() 
        {
            if (nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueId = Guid.NewGuid().ToString();
                nodes.Add(rootNode);
            }
            OnValidate();
        }
#endif

        private void OnValidate() 
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup.Add(node.uniqueId, node);
            }
        }   

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (string childID in parentNode.children)
            {
                if (nodeLookup.ContainsKey(childID)) result.Add(nodeLookup[childID]);
            }
            return result;
        }

        public void CreateNode(DialogueNode parentNode, Vector2 position)
        {
            DialogueNode newNode = new DialogueNode();
            newNode.uniqueId = Guid.NewGuid().ToString();
            newNode.rect.position = position;
            parentNode.children.Add(newNode.uniqueId);
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            PruneChildren(nodeToDelete);
        }

        private void PruneChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.uniqueId);
            }
        }
    }
}
