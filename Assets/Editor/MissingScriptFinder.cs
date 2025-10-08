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
            int goCount = 0;
            int componentsCount = 0;
            int missingCount = 0;

            foreach (GameObject go in GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                goCount++;
                Component[] components = go.GetComponents<Component>();

                for (int i = 0; i < components.Length; i++)
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
