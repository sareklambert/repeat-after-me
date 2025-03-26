namespace Plamb.Events
{
    /// <summary>
    /// A character was deleted.
    /// </summary>
    public class EventCharacterDied
    {
        public int CharacterID { get; private set; }

        public EventCharacterDied(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
