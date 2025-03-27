namespace Plamb.Character
{
    /// <summary>
    /// This struct acts as a wrapper for input snapshots. The attached timestamp is used when replaying inputs.
    /// </summary>
    public struct PlambRecord
    {
        public float Timestamp { get; private set; }
        public PlambInputSnapshot Input { get; private set; }
        
        public PlambRecord(float timestamp, PlambInputSnapshot plambInputSnapshot)
        {
            Timestamp = timestamp;
            Input = plambInputSnapshot;
        }
    }
}
