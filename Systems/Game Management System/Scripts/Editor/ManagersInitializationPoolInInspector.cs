using UnityEditor;

namespace PropellerCap
{
    [CustomEditor(typeof(ManagersInitializationPool))]
    public class ManagersInitializationPoolInInspector : Editor
    {
        ManagersInitializationPool managersPool;

        private void OnEnable()
        {
            managersPool = (ManagersInitializationPool)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("The first manager in the initialization pool should be the DebuggerManager to allow other managers to log information." +
                " The recommended order is:" +
                "\n Debugger Manager" +
                "\n Event Manager" +
                "\n Game Manager" +
                "\n Scene Loader" +
                "\n Sound Manager \n" +
                "And any other managers after that.", MessageType.Info);

            //TODO check that all items in teh list have a component with ManagerBase

            base.OnInspectorGUI();
        }
    }
}
