using System.Linq;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using GameEnvironment.Controller;
using Tool.Popup;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tool.Inspector
{
    [CustomEditor(typeof(PlaceHolder))]
    public class PlaceHolderInspector : BaseInspector
    {
        const string ID = "Id";
        const string CONTENT_ID = "Content Id";
        const string ADD = "Add";
        const string CHANGE = "Change";
        const string NAME = "Name";
        const string SELECT = "Select";
        private AssetLibrary m_library;
        private Label m_contentIdLabel;
        private Button m_addButton;
        private SerializedProperty m_contentProperty;
        private VisualElement m_contentViewElement;


        public override VisualElement CreateInspectorGUI()
        {
            m_inspector = new VisualElement()
            {
                style =
                {
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f)
                }
            };

            if (m_contentProperty == null)
            {
                m_contentProperty = serializedObject.FindProperty("m_contentId");
                m_inspector.TrackPropertyValue(m_contentProperty, val =>
                {
                    m_contentIdLabel.text = $"{CONTENT_ID}: {m_contentProperty.stringValue}";
                    m_addButton.text = CHANGE;
                    DrawContent(m_inspector, m_contentProperty.stringValue);
                });
            }

            PlaceHolder placeHolder = (PlaceHolder)target;

            m_inspector.Add(new Label($"{ID}: {placeHolder.Id}")
            {
                style =
                {
                    marginTop = 10,
                    marginLeft = 5,
                    width = 200,
                    height = 15,
                    alignSelf = Align.FlexStart
                }
            });

            var contentRow = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexShrink = 1f,
                    marginTop = 5,
                    marginRight = 5
                }
            };

            m_contentIdLabel = new Label($"{CONTENT_ID}: {placeHolder.ContentId}")
            {
                style =
                {
                    marginTop = 5,
                    marginLeft = 5,
                    width = 200,
                    height = 15,
                    flexGrow = 1
                }
            };

            contentRow.Add(m_contentIdLabel);

            m_addButton = new Button(AddContent)
            {
                style =
                {
                    marginBottom = 5,
                    backgroundColor = new Color(0.15f, 0.47f, 0.59f, 1),
                    width = 70,
                    height = 20
                }
            };

            m_addButton.text = (string.IsNullOrEmpty(placeHolder.ContentId)) ? ADD : CHANGE;
            contentRow.Add(m_addButton);

            m_inspector.Add(contentRow);

            if (!string.IsNullOrEmpty(placeHolder.ContentId))
            {
                DrawContent(m_inspector, placeHolder.ContentId);
            }

            return m_inspector;
        }

        private void AddContent()
        {
            if (m_library == null)
                m_library = LibraryUtility.LoadLibrary();

            var popup = CreateInstance(typeof(AddContentToolPopup)) as AddContentToolPopup;
            popup.OnComplete += AddBehaviourCallback;

            popup.ShowUtility();

            void AddBehaviourCallback(AddTemplateResult<BuildingTemplate> result)
            {
                popup.OnComplete -= AddBehaviourCallback;
                if (!result.Success)
                    return;


                serializedObject.ApplyModifiedProperties();
                var holder = (PlaceHolder)target;
                holder.SetContentId(result.Entity.Id);

                EditorApplication.QueuePlayerLoopUpdate();
            }

            popup.InitPopup(m_library.Buildings);
        }

        private void DrawContent(VisualElement root, BuildingTemplate template)
        {
            m_contentViewElement = new VisualElement()
            {
                style =
                {
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    marginBottom = 5,
                    backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f)
                }
            };

            var contentRow = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexShrink = 1f,
                    marginTop = 5
                }
            };

            contentRow.Add(new Label($"{NAME}: {template.Name}")
            {
                style =
                {
                    marginTop = 5,
                    marginLeft = 5,
                    width = 200,
                    height = 15,
                    flexGrow = 1
                }
            });


            contentRow.Add(new Button(() => { LibraryUtility.ViewInProject(template); })
            {
                text = SELECT,
                style =
                {
                    marginBottom = 5,
                    backgroundColor = new Color(0.15f, 0.47f, 0.59f, 1),
                    width = 70,
                    height = 20
                }
            });

            m_contentViewElement.Add(contentRow);

            root.Add(m_contentViewElement);
        }

        private void DrawContent(VisualElement root, string contentId)
        {
            if (m_library == null)
                m_library = LibraryUtility.LoadLibrary();

            if (m_library.TryGetBuildingTemplate(contentId, out var template))
            {
                DrawContent(root, template);
            }
        }
    }
}