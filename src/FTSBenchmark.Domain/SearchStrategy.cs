namespace FTSBenchmark.Domain;

public enum SearchStrategy
{
    Like = 0,
    Contains = 1,
    MatchAgainst = 2,
    LikeNoFront = 3,
    InMemory = 4,
    TrigramPg = 5,
    Redis = 6,
}