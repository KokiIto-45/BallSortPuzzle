#if UNITY_EDITOR
namespace MyApp.MyBSP.Menu
{
    using UnityEditor;
    using UnityEngine;

    public class BSPMenu : MainMenu
    {
        #region GameManager
        [MenuItem(Globals.RootName + "/" + Globals.PROJECT_NAME + "/BSP GameManager/" + MainMenuItem.Add)]
        public static void AddGameManager()
        {
            var node = FindObjectOfType<BSPGameManager>();
            if (node == null)
            {
                if (Selection.objects != null && Selection.objects.Length > 0)
                {
                    Selection.objects = new Object[] { Selection.objects[0] };
                    AttachComponentToSelection<BSPGameManager>();
                }
                else
                {
                    AddComponentToObject<BSPGameManager>("BSP GameManager");
                }
            }
            else
            {
                Selection.objects = new Object[] { node };
            }
        }
        [MenuItem(Globals.RootName + "/" + Globals.PROJECT_NAME + "/BSP GameManager/" + MainMenuItem.RemoveAll)]
        public static void RemoveGameManager()
        {
            RemoveComponentFromSelection<BSPGameManager>();
        }
        #endregion
        #region Basket
        [MenuItem(Globals.RootName + "/" + Globals.PROJECT_NAME + "/Basket/" + "Add basket")]
        public static void AddBasket()
        {
            var nodes = FindObjectsOfType<BSPGameManager>();
            if (nodes == null || nodes.Length == 0)
            {
                Debug.LogWarning("First add BSP GameManager");
                return;
            }
            foreach (var node in nodes)
            {
                if (node == null) continue;
                node.AddBasket();
            }
        }
        [MenuItem(Globals.RootName + "/" + Globals.PROJECT_NAME + "/Basket/" + "Remove basket")]
        public static void RemoveBasket()
        {
            var nodes = FindObjectsOfType<BSPGameManager>();
            foreach (var node in nodes)
            {
                if (node == null) continue;
                node.RemoveBasket();
            }
        }
        #endregion
    }
}
#endif
