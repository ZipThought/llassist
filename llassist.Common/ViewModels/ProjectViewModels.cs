namespace llassist.Common.ViewModels;

public class CreateEditProjectViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ProjectViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IList<ArticleViewModel> Articles { get; set; } = [];
    public ResearchQuestionsViewModel ResearchQuestions { get; set; } = new ResearchQuestionsViewModel();
}

public class ArticleViewModel
{
	public string Title { get; set; } = string.Empty;
	public string Authors { get; set; } = string.Empty;
	public int Year { get; set; } = 0;
	public string Abstract { get; set; } = string.Empty;
	public bool MustRead { get; set; } = false;
	public IList<RelevanceViewModel> Relevances { get; set; } = [];
}

public class RelevanceViewModel
{
	public string Question { get; set; } = string.Empty;
	public double RelevanceScore { get; set; } = 0;
	public bool IsRelevant { get; set; } = false;
}

public class ResearchQuestionsViewModel
{
	public IList<string> Questions { get; set; } = [];
}

public class UploadResultViewModel
{
	public string Id { get; set; } = string.Empty;
}

public class ProcessResultViewModel
{
	public int Progress { get; set; }
	public IList<ArticleViewModel> ProcessedArticles { get; set; } = [];
}