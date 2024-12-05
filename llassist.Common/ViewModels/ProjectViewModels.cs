namespace llassist.Common.ViewModels;

public class CreateEditProjectViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AddEditDefinitionViewModel
{
    public string Definition { get; set; } = string.Empty;
}


public class AddEditResearchQuestionViewModel
{
    public string Text { get; set; } = string.Empty;
    public IList<string> Definitions { get; set; } = [];
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
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public string Abstract { get; set; } = string.Empty;
    public string DOI { get; set; } = string.Empty;
    
    public string Link { get; set; } = string.Empty;
    public bool MustRead { get; set; } = false;
    public IList<string> Topics { get; set; } = [];
    public IList<string> Entities { get; set; } = [];
    public IList<string> Keywords { get; set; } = [];
    public IList<RelevanceViewModel> Relevances { get; set; } = [];
}

public class RelevanceViewModel
{
    public string Question { get; set; } = string.Empty;
    public double RelevanceScore { get; set; } = 0;
    public bool IsRelevant { get; set; } = false;
    public string RelevanceReason { get; set; } = string.Empty;
    public bool IsContributing { get; set; } = false;
    public double ContributionScore { get; set; } = 0;
    public string ContributionReason { get; set; } = string.Empty;
}

public class ResearchQuestionsViewModel
{
    public IList<string> Definitions { get; set; } = [];
    public IList<QuestionViewModel> Questions { get; set; } = [];
}

public class QuestionViewModel
{
    public string Text { get; set; } = string.Empty;
    public IList<string> Definitions { get; set; } = [];
}

public class UploadResultViewModel
{
    public string Id { get; set; } = string.Empty;
}

public class ProcessResultViewModel
{
    public string JobId { get; set; } = string.Empty;
    public int Progress { get; set; }
    public IList<ArticleViewModel> ProcessedArticles { get; set; } = [];
}
