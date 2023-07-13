using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace PropellerCap
{
    public static class VisualEffects
    {
        public static GameObject SpawnVFX(VisualEffectClip vfxClip, Transform trans)
        {
            return _SpawnVFX(vfxClip, trans.position, trans.rotation, trans.forward);
        }
        public static GameObject SpawnVFX(VisualEffectClip vfxClip, Vector3 position, Vector3 normal)
        {
            return _SpawnVFX(vfxClip, position, Quaternion.identity, normal);
        }

        private static GameObject _SpawnVFX(VisualEffectClip vfxClip, Vector3 position, Quaternion rotation, Vector3 normal)
        {
            if (vfxClip == null)
            {
                Debug.LogWarning("vfx clip is null.");
                return null;
            }

            if (vfxClip.vfxPrefab == null)
            {
                Debug.LogWarning("No VFX prefab assigned to clip : " + vfxClip.vfxName);
                return null;
            }

            GameObject obj = GameObject.Instantiate(vfxClip.vfxPrefab);

            //Set the position of the VFX
            obj.transform.position = position + vfxClip.spawnOffset;
            obj.transform.rotation = rotation * vfxClip.rotationOffset;
            obj.transform.LookAt(obj.transform.position + normal);

            return obj;
        }
    }
}