namespace Plamb.Events
{
    /// <summary>
    /// A character jumped.
    /// </summary>
    public class EventPlayerJump
    {
        public int CharacterID { get; private set; }

        public EventPlayerJump(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
