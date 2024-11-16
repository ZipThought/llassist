using llassist.Common.Models;
using llassist.Common.Models.Library;
using llassist.Common.ViewModels;
using System.Text.Json;

namespace llassist.Common.Mappers;

public class ModelMappers
{
    public static List<ArticleKeySemantic> ToArticleKeySemantics(Ulid articleId, KeySemantics keySemantics)
    {
        var keySemanticsList = new List<ArticleKeySemantic>();
        keySemanticsList.AddRange(ToArticleKeySemantic(articleId, keySemantics.Topics, ArticleKeySemantic.TypeTopic, keySemanticsList.Count));
        keySemanticsList.AddRange(ToArticleKeySemantic(articleId, keySemantics.Entities, ArticleKeySemantic.TypeEntity, keySemanticsList.Count));
        keySemanticsList.AddRange(ToArticleKeySemantic(articleId, keySemantics.Keywords, ArticleKeySemantic.TypeKeyword, keySemanticsList.Count));
        return keySemanticsList;
    }

    private static List<ArticleKeySemantic> ToArticleKeySemantic(Ulid articleId, string[] values, string type, int indexOffset)
    {
        return values.Select((entity, index) => new ArticleKeySemantic
        {
            ArticleId = articleId,
            KeySemanticIndex = indexOffset + index,
            Type = type,
            Value = entity,
        }).ToList();
    }

    public static List<ArticleRelevance> ToArticleRelevances(Ulid articleId, Ulid jobId, IList<Relevance> relevances)
    {
        return relevances.Select((relevance, index) => new ArticleRelevance
        {
            ArticleId = articleId,
            EstimateRelevanceJobId = jobId,
            RelevanceIndex = index,
            Question = relevance.Question,
            RelevanceScore = relevance.RelevanceScore,
            ContributionScore = relevance.ContributionScore,
            IsRelevant = relevance.IsRelevant,
            IsContributing = relevance.IsContributing,
            RelevanceReason = relevance.RelevanceReason,
            ContributionReason = relevance.ContributionReason
        }).ToList();
    }

    public static ProjectViewModel ToProjectViewModel(Project project)
    {
        var projectViewModel = new ProjectViewModel
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Description = project.Description,
            Articles = ToArticleViewModels(project.Articles),
            ResearchQuestions = ToResearchQuestionsViewModel(project.ProjectDefinitions, project.ResearchQuestions)
        };

        return projectViewModel;
    }

    private static List<ArticleViewModel> ToArticleViewModels(ICollection<Article> articles)
    {
        return articles.Select(article => ToArticleViewModel(article)).ToList();
    }

    public static ArticleViewModel ToArticleViewModel(Article article)
    {
        return ToArticleViewModel(article, article.ArticleRelevances);
    }

    public static ArticleViewModel ToArticleViewModel(Article article, ICollection<ArticleRelevance> articleRelevances)
    {
        return new ArticleViewModel
        {
            Title = article.Title,
            Authors = article.Authors,
            Year = article.Year,
            Abstract = article.Abstract,
            MustRead = article.MustRead,
            Relevances = ToRelevanceViewModels(articleRelevances)
        };
    }

    private static List<RelevanceViewModel> ToRelevanceViewModels(ICollection<ArticleRelevance> relevances)
    {
        return relevances.Select(relevance => new RelevanceViewModel
        {
            Question = relevance.Question,
            RelevanceScore = relevance.RelevanceScore,
            IsRelevant = relevance.IsRelevant,
        }).ToList();
    }

    public static ResearchQuestionsViewModel ToResearchQuestionsViewModel(
        ICollection<ProjectDefinition> projectDefinitions, 
        ICollection<ResearchQuestion> researchQuestions)
    {
        return new ResearchQuestionsViewModel
        {
            Definitions = projectDefinitions.Select(definition => definition.Definition).ToList(),
            Questions = researchQuestions.Select(question => new QuestionViewModel
            {
                Text = question.QuestionText,
                Definitions = question.QuestionDefinitions.Select(definition => definition.Definition).ToList()
            }).ToList()
        };
    }

    public static ICollection<Snapshot> ToSnapshots(
        Ulid EstimateRelevanceJobId,
        ICollection<ProjectDefinition> projectDefinitions, 
        ICollection<ResearchQuestion> researchQuestions)
    {
        var snapshots = projectDefinitions.Select(definition => new Snapshot
        {
            EntityType = Snapshot.EntityTypeProjectDefinition,
            EntityId = definition.Id,
            SerializedEntity = JsonSerializer.Serialize(definition),
            EstimateRelevanceJobId = EstimateRelevanceJobId
        }).ToList();

        snapshots.AddRange(researchQuestions.Select(question => new Snapshot
        {
            EntityType = Snapshot.EntityTypeResearchQuestion,
            EntityId = question.Id,
            SerializedEntity = JsonSerializer.Serialize(question),
            EstimateRelevanceJobId = EstimateRelevanceJobId
        }));

        foreach (var question in researchQuestions)
        {
            snapshots.AddRange(question.QuestionDefinitions.Select(definition => new Snapshot
            {
                EntityType = Snapshot.EntityTypeQuestionDefinition,
                EntityId = definition.Id,
                SerializedEntity = JsonSerializer.Serialize(definition),
                EstimateRelevanceJobId = EstimateRelevanceJobId
            }));
        }

        return snapshots;
    }

    public static List<ResearchQuestionDTO> ToResearchQuestionDTOList(ICollection<ProjectDefinition> projectDefinitions, ICollection<ResearchQuestion> researchQuestions)
    {
        var researchQuestionList = new List<ResearchQuestionDTO>();
        foreach (var question in researchQuestions)
        {
            researchQuestionList.Add(new ResearchQuestionDTO
            {
                Question = question.QuestionText,
                CombinedDefinitions = projectDefinitions.Select(d => d.Definition).Concat(
                    question.QuestionDefinitions.Select(d => d.Definition)).ToArray(),
            });
        }
        return researchQuestionList;
    }

    // Catalog mapping
    public static CatalogViewModel ToCatalogViewModel(Catalog catalog)
    {
        return new CatalogViewModel
        {
            Id = catalog.Id.ToString(),
            Name = catalog.Name,
            Description = catalog.Description,
            Owner = catalog.Owner,
            CreatedAt = catalog.CreatedAt,
            Entries = catalog.Entries.Select(ToEntryViewModel).ToList()
        };
    }

    // Entry mapping
    public static EntryViewModel ToEntryViewModel(Entry entry)
    {
        return new EntryViewModel
        {
            Id = entry.Id.ToString(),
            EntryType = entry.EntryType,
            Title = entry.Title,
            Description = entry.Description,
            Citation = entry.Citation,
            Source = entry.Source,
            Identifier = entry.Identifier,
            CreatedAt = entry.CreatedAt,
            PublishedAt = entry.PublishedAt,
            Resources = entry.Resources.Select(ToResourceViewModel).ToList(),
            Labels = entry.Labels.Select(l => l.Label.Name).ToList(),
            Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.Metadata) 
                ?? new Dictionary<string, object>()
        };
    }

    // Resource mapping
    public static ResourceViewModel ToResourceViewModel(Resource resource)
    {
        return new ResourceViewModel
        {
            Id = resource.Id.ToString(),
            Name = resource.Name,
            Type = resource.Type,
            ContentType = resource.ContentType,
            Size = resource.Size
        };
    }

    // Category tree mapping
    public static CategoryTreeViewModel ToCategoryTreeViewModel(string schemaType, IEnumerable<Category> rootCategories)
    {
        return new CategoryTreeViewModel
        {
            SchemaType = schemaType,
            RootCategories = rootCategories
                .Where(c => c.ParentId == null && c.SchemaType == schemaType)
                .Select(ToCategoryViewModel)
                .ToList()
        };
    }

    // Category mapping with recursive children
    public static CategoryViewModel ToCategoryViewModel(Category category)
    {
        return new CategoryViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name,
            Description = category.Description,
            Path = category.Path,
            Depth = category.Depth,
            SchemaType = category.SchemaType,
            ParentId = category.ParentId?.ToString(),
            Children = category.Children.Select(ToCategoryViewModel).ToList(),
            EntryIds = category.Entries.Select(ce => ce.EntryId.ToString()).ToList()
        };
    }

    // Category path mapping for navigation
    public static CategoryPathViewModel ToCategoryPathViewModel(Category category)
    {
        var breadcrumbs = new List<CategoryBreadcrumbViewModel>();
        var current = category;
        var pathParts = new List<string>();
        
        // Build breadcrumbs and path parts from current category up to root
        while (current != null)
        {
            breadcrumbs.Insert(0, new CategoryBreadcrumbViewModel
            {
                Id = current.Id.ToString(),
                Name = current.Name,
                Depth = current.Depth
            });
            pathParts.Insert(0, current.Name);
            current = current.Parent;
        }

        return new CategoryPathViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name,
            Path = "/" + string.Join("/", pathParts),
            Breadcrumbs = breadcrumbs
        };
    }

    // Label mapping
    public static LabelViewModel ToLabelViewModel(Label label)
    {
        return new LabelViewModel
        {
            Id = label.Id.ToString(),
            Name = label.Name,
            Description = label.Description,
            Color = label.Color
        };
    }

    // Helper method to build category paths
    public static string BuildCategoryPath(Category category)
    {
        var pathParts = new List<string>();
        var current = category;
        
        while (current != null)
        {
            pathParts.Insert(0, current.Name);
            current = current.Parent;
        }
        
        return "/" + string.Join("/", pathParts);
    }

    // Helper method to calculate category depth
    public static int CalculateCategoryDepth(Category category)
    {
        int depth = 0;
        var current = category;
        
        while (current.Parent != null)
        {
            depth++;
            current = current.Parent;
        }
        
        return depth;
    }

    // Map a list of categories by schema type
    public static IDictionary<string, CategoryTreeViewModel> ToCategoryTreeViewModels(IEnumerable<Category> categories)
    {
        return categories
            .GroupBy(c => c.SchemaType)
            .ToDictionary(
                g => g.Key,
                g => ToCategoryTreeViewModel(g.Key, g.Where(c => c.ParentId == null))
            );
    }

    // Map a flat list of categories with their paths
    public static IList<CategoryPathViewModel> ToCategoryPathViewModels(IEnumerable<Category> categories)
    {
        return categories
            .Select(ToCategoryPathViewModel)
            .OrderBy(c => c.Path)
            .ToList();
    }

    // Map entry with its category paths
    public static EntryWithCategoriesViewModel ToEntryWithCategoriesViewModel(Entry entry)
    {
        var viewModel = new EntryWithCategoriesViewModel
        {
            Id = entry.Id.ToString(),
            EntryType = entry.EntryType,
            Title = entry.Title,
            Description = entry.Description,
            Citation = entry.Citation,
            Source = entry.Source,
            Identifier = entry.Identifier,
            CreatedAt = entry.CreatedAt,
            PublishedAt = entry.PublishedAt,
            Resources = entry.Resources.Select(ToResourceViewModel).ToList(),
            Labels = entry.Labels.Select(l => l.Label.Name).ToList(),
            Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.Metadata) 
                ?? new Dictionary<string, object>(),
        };

        // Group categories by schema type
        foreach (var group in entry.Categories.GroupBy(ce => ce.Category.SchemaType))
        {
            viewModel.Categories[group.Key] = group
                .Select(ce => ToCategoryPathViewModel(ce.Category))
                .ToList();
        }

        return viewModel;
    }

    // Map catalog with category trees
    public static CatalogWithCategoriesViewModel ToCatalogWithCategoriesViewModel(Catalog catalog)
    {
        return new CatalogWithCategoriesViewModel
        {
            Id = catalog.Id.ToString(),
            Name = catalog.Name,
            Description = catalog.Description,
            Owner = catalog.Owner,
            CreatedAt = catalog.CreatedAt,
            Entries = catalog.Entries.Select(ToEntryViewModel).ToList(),
            CategoryTrees = ToCategoryTreeViewModels(catalog.Categories)
        };
    }

    // Map search results with category paths
    public static SearchResultViewModel ToSearchResultViewModel(
        Entry entry, 
        IEnumerable<CategoryPathViewModel> categoryPaths)
    {
        return new SearchResultViewModel
        {
            Entry = ToEntryViewModel(entry),
            CategoryPaths = categoryPaths.ToList()
        };
    }
}
