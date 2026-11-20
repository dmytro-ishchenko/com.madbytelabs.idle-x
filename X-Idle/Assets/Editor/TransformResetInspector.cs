using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MadByteLabs.EditorTools
{
    [CustomEditor(typeof(Transform))]
    [CanEditMultipleObjects]
    internal sealed class TransformResetInspector : Editor
    {
        // Keep Unity's rotation fields and proportional scaling controls.
        private static readonly Type BuiltInInspector =
            typeof(Editor).Assembly.GetType("UnityEditor.TransformInspector");

        private Editor builtInEditor;

        [Flags]
        private enum ResetFields
        {
            Position = 1,
            Rotation = 2,
            Scale = 4,
            All = Position | Rotation | Scale
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (BuiltInInspector != null)
                CreateCachedEditor(targets, BuiltInInspector, ref builtInEditor);

            var root = new VisualElement();
            root.Add(new IMGUIContainer(DrawTransformFields));

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.flexWrap = Wrap.Wrap;
            buttons.Add(new Button(() => ResetTransform(ResetFields.Position, "Reset Position")) { text = "Reset Position" });
            buttons.Add(new Button(() => ResetTransform(ResetFields.Rotation, "Reset Rotation")) { text = "Reset Rotation" });
            buttons.Add(new Button(() => ResetTransform(ResetFields.Scale, "Reset Scale")) { text = "Reset Scale" });
            buttons.Add(new Button(() => ResetTransform(ResetFields.All, "Reset Transform")) { text = "Reset All" });
            root.Add(buttons);

            return root;
        }

        private void DrawTransformFields()
        {
            if (builtInEditor != null)
                builtInEditor.OnInspectorGUI();
            else
                DrawDefaultInspector();
        }

        private void OnDisable()
        {
            if (builtInEditor != null)
                DestroyImmediate(builtInEditor);
        }

        private void ResetTransform(ResetFields fields, string undoName)
        {
            serializedObject.Update();

            if ((fields & ResetFields.Position) != 0)
                serializedObject.FindProperty("m_LocalPosition").vector3Value = Vector3.zero;

            if ((fields & ResetFields.Rotation) != 0)
            {
                serializedObject.FindProperty("m_LocalRotation").quaternionValue = Quaternion.identity;
                var eulerHint = serializedObject.FindProperty("m_LocalEulerAnglesHint");
                if (eulerHint != null)
                    eulerHint.vector3Value = Vector3.zero;
            }

            if ((fields & ResetFields.Scale) != 0)
                serializedObject.FindProperty("m_LocalScale").vector3Value = Vector3.one;

            Undo.SetCurrentGroupName(undoName);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
