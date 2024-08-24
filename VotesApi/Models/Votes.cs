namespace VotesApi.Models
{
    public class Vote
    {
        public required string Candidate { get; set; }
        public int Votes { get; set; }
        public required string Gender { get; set; }
    }
}
