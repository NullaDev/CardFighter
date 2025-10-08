#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MissingScriptFinder
    {
        [MenuItem("Tools/Find Missing Scripts in Scene")]
        public static void FindMissingScripts()
        {
            var goCount = 0;
            var componentsCount = 0;
            var missingCount = 0;

            foreach (var go in GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                goCount++;
                var components = go.GetComponents<Component>();

                for (var i = 0; i < components.Length; i++)
                {
                    componentsCount++;
                    if (components[i] == null)
                    {
                        missingCount++;
                        Debug.LogWarning($"Missing script found on: {GetFullPath(go)}", go);
                    }
                }
            }

            Debug.Log($"Searched {goCount} GameObjects, {componentsCount} components, found {missingCount} missing scripts.");
        }

        private static string GetFullPath(GameObject go)
        {
            return go.transform.parent == null
                ? go.name
                : GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
        }
    }
}
#endif
