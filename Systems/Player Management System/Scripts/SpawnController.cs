using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SpawnController
    {
        List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();


        public void RemoveNewSpawnPoint(SpawnPoint spawnPoint)
        {
            if (_spawnPoints.Contains(spawnPoint) == false)
            {
                Debugger.LogError($"SpawnController does not contain the spawnPoint : {spawnPoint.gameObject.name}");
                return;
            }
            _spawnPoints.Remove(spawnPoint);
        }

        public void AddNewSpawnPoint(PlayerSpawnPoint spawnPoint)
        {
            if (_spawnPoints.Contains(spawnPoint))
            {
                Debugger.LogError($"SpawnController already contains the spawnPoint : {spawnPoint.gameObject.name}");
                return;
            }
            _spawnPoints.Add(spawnPoint);
        }

        public SpawnPoint GetValidSpawnPoint()
        {
            foreach (var item in _spawnPoints)
            {
                if (item.PointIsValid())
                    return item;
            }
            Debugger.LogWarning("No valid spawn points.");
            return null;
        }
        public List<SpawnPoint> GetValidPoints()
        {
            var toReturn = new List<SpawnPoint>();
            foreach (var item in _spawnPoints)
            {
                toReturn.Add(item);
            }
            if (toReturn.Count == 0)
                Debugger.LogError("There are no valid spawn points registered.");
            return toReturn;
        }

        public void ApplySpawnPosRotToObject(GameObject targetObject, SpawnPoint spawnPoint)
        {
            targetObject.transform.position = spawnPoint.GetSpawnPosition();
            targetObject.transform.rotation = spawnPoint.GetSpawnRotation();
        }
    }
}