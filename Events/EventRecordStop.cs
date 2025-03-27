namespace Plamb.Events
{
    /// <summary>
    /// Recording of a clone has stopped.
    /// </summary>
    public class EventRecordStop
    {
        public int CharacterID { get; private set; }
    
        public EventRecordStop(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
