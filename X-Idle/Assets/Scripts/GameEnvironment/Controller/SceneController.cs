using System;
using GameEnvironment.Factory;
using UnityEngine;

namespace GameEnvironment.Controller
{
    internal class SceneController : MonoBehaviour, ISceneController
    {
        [SerializeField] private PlaceholderRoot m_placeholderRoot;
        private IEnvironmentFactory m_factory;
        public event Action OnSceneInitialized;

        public void InitContent(IEnvironmentFactory factory)
        {
            m_factory = factory;

            foreach (var placeHolder in m_placeholderRoot.PlaceHolders)
            {
                var view = m_factory.GetView(placeHolder.ContentId);
                view.GameObject.transform.SetParent(placeHolder.transform);
                view.GameObject.transform.localPosition = Vector3.zero;
                view.GameObject.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}