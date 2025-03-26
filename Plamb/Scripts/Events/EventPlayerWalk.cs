namespace Plamb.Events
{
    /// <summary>
    /// A character walked.
    /// </summary>
    public class EventPlayerWalk
    {
        public int CharacterID { get; private set; }
        public float Speed { get; private set; }

        public EventPlayerWalk(int characterID, float speed)
        {
            CharacterID = characterID;
            Speed = speed;
        }
    }
}
