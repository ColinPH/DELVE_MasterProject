using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public struct PositionSample
    {
        public float x, y, z;

        public PositionSample(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 position => new Vector3(x, y, z);

        /*public void Write(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(Position.x);
            writer.Write(Position.y);
            writer.Write(Position.z);
        }

        public static PositionSample Read(BinaryReader reader)
        {
            float time = reader.ReadSingle();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            Vector3 position = new Vector3(x, y, z);
            return new PositionSample(time, position);
        }*/
    }
}