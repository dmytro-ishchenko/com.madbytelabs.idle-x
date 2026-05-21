using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tool.Popup
{
    public class BaseToolPopup: EditorWindow
    {
        protected string m_filter;
        VisualElement m_title;

        const string CLEAN = "Clean";
        VisualElement m_scroll;

        protected void DrawTitle(string titleName) {
            m_title = new VisualElement() {
                style = {
                    height = 30,
                    marginTop = 10,
                    alignContent = Align.Center
                }
            };

            m_title.Add(new Label(titleName) {
                style = {
                    height = 30,
                    alignSelf = Align.Center,
                    fontSize = 16,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            rootVisualElement.Add(m_title);
        }

        protected void DrawSearch() {
            var row = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    flexShrink = 0f,
                    height = 34,
                    marginTop = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    backgroundColor = Color.black
                }
            };

            row.Add(new Label("Search") {
                style = {
                    marginTop = 7,
                    marginLeft = 10,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 14
                }
            });

            var searchLabel = new TextField() {
                style = {
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 7,
                    marginBottom = 7,
                    position = Position.Relative,
                    flexGrow = 1
                }
            };

            searchLabel.value = m_filter;

            searchLabel.RegisterCallback<ChangeEvent<string>>(e => {
                SearchFilter(e.newValue);
            });

            row.Add(searchLabel);
            row.Add(new Button(() => {
                searchLabel.value = string.Empty;
            }) {
                text = CLEAN, style = {
                    width = 70,
                    height = 20,
                    backgroundColor = new Color(0.15f, 0.47f, 0.59f, 1),
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginRight = 5,
                    marginTop = 7
                }
            });
            rootVisualElement.Add(row);

            EditorApplication.delayCall += () => searchLabel.Focus();
        }

        protected virtual void SearchFilter(string prompt) { }

        protected void DrawElementsList<T>(IList<T> source, Func<T, string> getName, Action<T> onClick) {
            if (m_scroll != null)
                rootVisualElement.Remove(m_scroll);

            m_scroll = new ScrollView(ScrollViewMode.Vertical);
            m_scroll.style.marginTop = 10;
            m_scroll.style.marginBottom = 10;
            m_scroll.style.marginLeft = 10;
            m_scroll.style.marginRight = 5;
            m_scroll.style.height = 300;

            var list = string.IsNullOrEmpty(m_filter)
                ? source
                : source.Where(e => MatchesAllTerms(getName(e), m_filter)).ToList();

            foreach (var element in list)
                m_scroll.Add(CreateDefaultElementView(getName(element), () => onClick?.Invoke(element)));

            rootVisualElement.Add(m_scroll);
        }

        private VisualElement CreateDefaultElementView(string label, Action click) {
            var row = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row,
                    height = 30,
                    marginTop = 5,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f)
                }
            };

            var nameLabel = new Label($"  {label}") {
                style = {
                    marginTop = 10,
                    marginLeft = 10,
                    fontSize = 12
                }
            };
            var nameContainer = new VisualElement() {
                style = { height = 30 }
            };
            nameContainer.Add(nameLabel);
            row.Add(nameContainer);

            row.Add(new Button(click) {
                text = "Add",
                style = {
                    position = Position.Absolute,
                    right = 10,
                    width = 80,
                    height = 20,
                    marginTop = 5,
                    backgroundColor = new Color(0.09f, 0.32f, 0.03f, 1f)
                }
            });

            return row;
        }

        private bool MatchesAllTerms(string text, string filter) {
            if (string.IsNullOrWhiteSpace(filter))
                return true;

            text = text.ToLower();
            var terms = filter.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return terms.All(term => text.Contains(term));
        }
    }
}