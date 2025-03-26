namespace Plamb.Events
{
    /// <summary>
    /// A character's loop was reset.
    /// </summary>
    public class EventPlayerLoopReset
    {
        public int CharacterID { get; private set; }

        public EventPlayerLoopReset(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
