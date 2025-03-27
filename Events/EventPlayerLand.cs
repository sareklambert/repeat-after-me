namespace Plamb.Events
{
    /// <summary>
    /// A character landed.
    /// </summary>
    public class EventPlayerLand
    {
        public int CharacterID { get; private set; }
        public float VerticalVelocity { get; private set; }

        public EventPlayerLand(int characterID, float verticalVelocity)
        {
            CharacterID = characterID;
            VerticalVelocity = verticalVelocity;
        }
    }
}
