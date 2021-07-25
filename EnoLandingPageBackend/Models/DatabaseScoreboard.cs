namespace EnoLandingPageCore.Models
{
    /// <summary>
    /// A Class representing a scoreboard in the database.
    /// </summary>
    public class DatabaseScoreboard
    {
        /// <summary>
        /// Create a new DatabaseScoreboard
        /// </summary>
        public DatabaseScoreboard(long roundId, string scoreboardString)
        {
            this.roundId = roundId;
            this.scoreboardString = scoreboardString;
        }
        /// <summary>
        /// The roundId of the Scoreboard.
        /// </summary>
        public long roundId { get; set; }

        /// <summary>
        /// The String representation of this scoreboard.
        /// </summary>
        public string scoreboardString { get; set; }

    }
}
