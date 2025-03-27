namespace Plamb.Events
{
    /// <summary>
    /// A character interacted.
    /// </summary>
    public class EventPlayerInteract
    {
        public int CharacterID { get; private set; }

        public EventPlayerInteract(int characterID)
        {
            CharacterID = characterID;
        }
    }
}
