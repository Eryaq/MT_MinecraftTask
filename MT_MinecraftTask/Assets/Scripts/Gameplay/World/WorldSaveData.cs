using MT_MinecraftTask.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MT_MinecraftTask
{
    [Serializable]
    public class WorldSaveData
    {
        public int Seed;
        public List<WorldModification> Modifications = new();
        public Vector3 PlayerPosition;
        public Vector2 PlayerLookRotation;
        public Vector3 PlayerEulerAngles;
    }
}
