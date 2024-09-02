using System;

namespace Rehawk.Foundation.SceneHistory
{
    public struct SceneHistoryRecord : IEquatable<SceneHistoryRecord>
    {
        public string SceneName { get; set; }
        public int BuildIndex { get; set; }

        public static SceneHistoryRecord Empty
        {
            get
            {
                return new SceneHistoryRecord
                {
                    BuildIndex = -1
                };
            }
        }
        
        public bool Equals(SceneHistoryRecord other)
        {
            return SceneName == other.SceneName && BuildIndex == other.BuildIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneHistoryRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SceneName != null ? SceneName.GetHashCode() : 0) * 397) ^ BuildIndex;
            }
        }
        
        public static bool operator ==(SceneHistoryRecord recordA, SceneHistoryRecord recordB)
        {
            return Equals(recordA, recordB);
        }
        
        public static bool operator !=(SceneHistoryRecord recordA, SceneHistoryRecord recordB)
        {
            return !Equals(recordA, recordB);
        }
    }
}