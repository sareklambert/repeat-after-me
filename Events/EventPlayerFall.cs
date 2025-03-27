namespace Plamb.Events
{
    /// <summary>
    /// A character started falling.
    /// </summary>
    public class EventPlayerFall
    {
        public int CharacterID { get; private set; }
        public float VerticalVelocity { get; private set; }

        public EventPlayerFall(int characterID, float verticalVelocity)
        {
            CharacterID = characterID;
            VerticalVelocity = verticalVelocity;
        }
    }
}
