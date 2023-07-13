using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace PropellerCap
{
    public class LevelRecorder : LocalManagerAction
    {
        float _sampleIntervalInSec = 0.2f;
        private float nextSampleTime;

        bool _isSampling = false;
        List<PositionSample> positions = new List<PositionSample>();
        Transform playerTransform;
        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            base.InvokeLocalManagerAction(localManager);

            if (Managers.playerManager._recordLevelData == false)
                return;

            playerTransform = Player.PlayerObject.transform;
            nextSampleTime = Time.time;
            _isSampling = true;
        }

        private void Update()
        {
            if (_isSampling == false)
                return;

            if (Time.time >= nextSampleTime)
            {
                Vector3 pos = playerTransform.position;
                positions.Add(new PositionSample(pos.x, pos.y, pos.z));
                nextSampleTime += _sampleIntervalInSec;
            }
        }

        protected override void MonoDestroy()
        {
            //Debug.Log("______________________________Serialize Data");
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string folderPath = Application.persistentDataPath + "/Recordings";
            //Create recordings folder if it doesn't exist
            if (Directory.Exists(folderPath) == false)
                Directory.CreateDirectory(folderPath);

            string fileName = $"LevelData_{Metrics.sessionData.ID}_{m_localManager.LocalManagerName}_{timeStamp}.bin";
            string finalPath = folderPath + "/" + fileName;

            using (FileStream fileStream = new FileStream(finalPath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, positions);
            }
            //Debug.Log($"Positions amount {positions.Count}");
        }

    }
}