using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Props/" + nameof(ItemDropper))]
    public class ItemDropper : WorldObject
    {
        [Tooltip("Radius of the circle around the object on which the loots will be spawned")]
        [SerializeField] float _dropRadius = 1f;
        [Range(0f, 100f)]
        [SerializeField] float _dropChance = 50f;
        [SerializeField] Vector3 _spawnOffset = Vector3.zero;

        public void DropItems(List<GameObject> itemsToDrop)
        {
            if (Random.Range(0f, 100f) > _dropChance) 
                return;

            List<Vector3> posList = _GetPositionsOnCircle(transform.position + _spawnOffset, itemsToDrop.Count, _dropRadius);
            foreach (Vector3 pos in posList)
            {
                GameObject o = Instantiate(itemsToDrop[posList.IndexOf(pos)]);
                o.transform.position = pos;
            }
        }

        private List<Vector3> _GetPositionsOnCircle(Vector3 center, int amount, float radius)
        {
            List<Vector3> toReturn = new List<Vector3>();
            if (amount == 1)
            {
                toReturn.Add(center);
            }
            else
            {
                float angle = 360f / amount;
                for (int i = 0; i < amount; i++)
                {
                    Vector3 pos = center;
                    pos.x += Mathf.Cos(angle * i * Mathf.Deg2Rad) * radius;
                    pos.z += Mathf.Sin(angle * i * Mathf.Deg2Rad) * radius;
                    //Debug.Log("Angle is " + angle * i + " and x and y are equal to " + pos.x + " and " + pos.z);
                    toReturn.Add(pos);
                }
            }
            return toReturn;
        }
    }
}
