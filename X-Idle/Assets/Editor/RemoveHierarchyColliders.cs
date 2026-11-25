using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MadByteLabs.BuildingTools
{
    internal static class RemoveHierarchyColliders
    {
        private const string MenuPath = "GameObject/Remove Colliders in Hierarchy";
        private const string UndoLabel = "Remove Colliders in Hierarchy";

        [MenuItem(MenuPath, false, 49)]
        private static void Remove()
        {
            if (!CanRemove())
                return;

            GameObject root = Selection.activeGameObject;
            Collider[] colliders3D = root.GetComponentsInChildren<Collider>(includeInactive: true);
            Collider2D[] colliders2D = root.GetComponentsInChildren<Collider2D>(includeInactive: true);
            int total = colliders3D.Length + colliders2D.Length;

            if (total == 0)
            {
                Debug.Log($"No colliders found under '{root.name}'.", root);
                return;
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(UndoLabel);

            try
            {
                foreach (Collider collider in colliders3D)
                    if (collider)
                        Undo.DestroyObjectImmediate(collider);

                foreach (Collider2D collider in colliders2D)
                    if (collider)
                        Undo.DestroyObjectImmediate(collider);
            }
            finally
            {
                Undo.CollapseUndoOperations(undoGroup);
            }

            int remaining = root.GetComponentsInChildren<Collider>(true).Length
                + root.GetComponentsInChildren<Collider2D>(true).Length;

            if (remaining > 0)
                Debug.LogWarning($"Removed {total - remaining} colliders from '{root.name}'. Unity kept {remaining} colliders. Check the Console for details.", root);
            else
                Debug.Log($"Removed {total} colliders from '{root.name}' and its children.", root);
        }

        [MenuItem(MenuPath, true)]
        private static bool CanRemove()
        {
            GameObject root = Selection.activeGameObject;
            return root && !EditorUtility.IsPersistent(root)
                && root.scene.IsValid() && root.scene.isLoaded
                && PrefabStageUtility.GetPrefabStage(root) == null;
        }
    }
}
