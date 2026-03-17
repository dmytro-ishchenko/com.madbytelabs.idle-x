using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Tool.Inspector
{
    public class BaseInspector :Editor
    {
    private const string SCRIPT_PROPERTY_NAME = "m_Script";

    protected VisualElement m_inspector;

    protected void DoDrawDefaultIMGUIProperty(SerializedObject serializedObject, SerializedProperty property) {
        EditorGUI.BeginChangeCheck();
        serializedObject.Update();

        EditorGUILayout.PropertyField(property, true);

        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndChangeCheck();
    }

    protected void DrawBaseProperties(VisualElement container) {
        var iterator = serializedObject.GetIterator();
        var enterChildren = true;

        while (iterator.NextVisible(enterChildren)) {
            enterChildren = false;
            var propertyField = new PropertyField(iterator.Copy());
            if (iterator.name.Equals(SCRIPT_PROPERTY_NAME)) {
                propertyField.SetEnabled(false);
            }

            container.Add(propertyField);
        }
    }
    protected IEnumerable<SerializedProperty> GetChildren(SerializedProperty property) {
        property = property.Copy();
        var nextElement = property.Copy();
        bool hasNextElement = nextElement.Next(false);
        if (!hasNextElement) {
            nextElement = null;
        }

        property.NextVisible(true);
        while (true) {
            if ((SerializedProperty.EqualContents(property, nextElement))) {
                yield break;
            }

            yield return property;

            bool hasNext = property.NextVisible(false);
            if (!hasNext) {
                break;
            }
        }
    }
    }
}