using UnityEngine.SceneManagement;

namespace Common
{
    public static class Extensions
    {
        public static T GetComponent<T>(this Scene scene)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                foreach (var component in gameObject.GetComponents<T>())
                {
                    if (component is { } target)
                        return target;
                }
            }

            return default;
        }
    }
}