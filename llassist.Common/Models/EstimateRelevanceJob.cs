namespace llassist.Common.Models;

public class EstimateRelevanceJob : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string ModelName { get; set; } = string.Empty;
    public Ulid ProjectId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ResearchQuestionsSnapshot ResearchQuestions { get; set; } = new ResearchQuestionsSnapshot();
}

public class ResearchQuestionsSnapshot
{
    public IList<string> Definitions { get; set; } = [];
    public IList<QuestionSnapshot> Questions { get; set; } = [];
}

public class QuestionSnapshot
{
    public string Text { get; set; } = string.Empty;
    public IList<string> Definitions { get; set; } = [];
}
