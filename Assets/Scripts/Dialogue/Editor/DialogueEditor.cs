using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deleteNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;
        [NonSerialized] GUIStyle selectedNodeStyle;
        string selectedNodeId = null;

        const float CANVAS_SIZE = 4000;
        const float BACKGROUND_SIZE = 50;

        private void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged;
            
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("node0 on") as Texture2D;
            selectedNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null) 
            {   
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        [MenuItem("Window/Dialogue Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        [OnOpenAsset(1)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowWindow();
                return true;
            }
            return false;
        }

        private void OnGUI()
        {
            if (selectedDialogue != null)
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textCoords);

                DrawConnectionsAndNodes();

                EditorGUILayout.EndScrollView();

                HandleNodeCreationAndDeletion();
            }
            else
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }            
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                    selectedNodeId = draggingNode.uniqueId;
                }
                else 
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                    selectedNodeId = null;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null) 
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;                
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawConnectionsAndNodes()
        {
            // separate foreach to keep connections drawn under nodes
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);
            }
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node);
            }
        }

        private void DrawNode(DialogueNode node)
        {
            if(node.uniqueId == selectedNodeId) GUILayout.BeginArea(node.rect, selectedNodeStyle);
            else GUILayout.BeginArea(node.rect, nodeStyle);
            
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("NodeTextField");
            string newText = EditorGUILayout.TextField(node.text);
            if (node.uniqueId == selectedNodeId) EditorGUI.FocusTextInControl("NodeTextField");

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                node.text = newText;
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete Node")) deleteNode = node;
            if (GUILayout.Button("Add Node")) creatingNode = node;
            GUILayout.EndHorizontal();
            DrawLinkButtons(node);

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {                
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = new Vector2((endPosition.x - startPosition.x) * 0.8f, 0);
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, endPosition - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }

        private void HandleNodeCreationAndDeletion()
        {
            if (creatingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                selectedDialogue.CreateNode(creatingNode, Event.current.mousePosition + draggingOffset);
                creatingNode = null;
            }
            if (deleteNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Removed Dialogue Node");
                selectedDialogue.DeleteNode(deleteNode);
                deleteNode = null;
            }
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link")) linkingParentNode = node;
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel")) linkingParentNode = null;
            }
            else if (linkingParentNode.children.Contains(node.uniqueId))
            {
                if (GUILayout.Button("Unlink"))
                {
                    Undo.RecordObject(selectedDialogue, "Removed Dialogue Link");
                    linkingParentNode.children.Remove(node.uniqueId);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.children.Add(node.uniqueId);
                    linkingParentNode = null;
                }
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode returnNode = null;
            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.rect.Contains(point)) returnNode = node;
            }
            return returnNode;
        }
    }
}
