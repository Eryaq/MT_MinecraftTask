using MT_MiencraftTask.World;
using System;
using System.Collections.Generic;

namespace MT_MiencraftTask
{
    [Serializable]
    public class WorldSaveData
    {
        public List<WorldModification> Modifications = new();
    }
}
