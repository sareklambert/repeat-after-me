namespace Plamb.Events
{
    /// <summary>
    /// Delete a character with the given ID.
    /// </summary>
    public class EventDeleteCharacterByID
    {
        public int CharacterID { get; private set; }

        public EventDeleteCharacterByID(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
