namespace Plamb.Events
{
    /// <summary>
    /// Recording of a clone has started.
    /// </summary>
    public class EventRecordStart
    {
        public int CharacterID { get; private set; }

        public EventRecordStart(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
