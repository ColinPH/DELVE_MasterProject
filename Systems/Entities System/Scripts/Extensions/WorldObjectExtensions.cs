using System.Linq;
using UnityEngine;

namespace PropellerCap
{
    public static class WorldObjectExtensions
    {
        public static void DestroyMyObject(this WorldObject wObj)
        {
            wObj.DestroyMyObject();
        }

        public static void DestroyMyObject(this GameObject obj)
        {
            WorldObject[] wObjs = obj.GetComponents<WorldObject>();
            wObjs.ToList().AddRange(obj.GetComponentsInChildren<WorldObject>());

            if (wObjs.Count() == 0)
            {
                Debug.LogWarning($"When DestroyingMyObject. There seems to be no component inheriting from \"{nameof(WorldObject)}\" on/in the object \"{obj.name}\".");
                GameObject.Destroy(obj);
            }

            foreach (var item in wObjs)
            {
                item.DestroyMyObject();
            }
        }
    }
}