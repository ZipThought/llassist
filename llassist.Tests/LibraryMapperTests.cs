using llassist.Common.Mappers;
using llassist.Common.Models.Library;
using llassist.Common.ViewModels;
using Xunit;
using Assert = Xunit.Assert;
using System.Text.Json;

namespace llassist.Tests;

public class LibraryMapperTests
{
    [Fact]
    public void ToCatalogViewModel_MapsAllProperties()
    {
        // Arrange
        var catalog = new Catalog
        {
            Id = Ulid.NewUlid(),
            Name = "Test Catalog",
            Description = "Test Description",
            Owner = "Test Owner",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var viewModel = ModelMappers.ToCatalogViewModel(catalog);

        // Assert
        Assert.Equal(catalog.Id.ToString(), viewModel.Id);
        Assert.Equal(catalog.Name, viewModel.Name);
        Assert.Equal(catalog.Description, viewModel.Description);
        Assert.Equal(catalog.Owner, viewModel.Owner);
        Assert.Equal(catalog.CreatedAt, viewModel.CreatedAt);
    }

    [Fact]
    public void ToCategoryTreeViewModel_BuildsHierarchy()
    {
        // Arrange
        var catalogId = Ulid.NewUlid();
        var root = new Category 
        { 
            Id = Ulid.NewUlid(),
            Name = "Root",
            SchemaType = "subject",
            CatalogId = catalogId
        };
        
        var child = new Category
        {
            Id = Ulid.NewUlid(),
            Name = "Child",
            SchemaType = "subject",
            CatalogId = catalogId,
            Parent = root
        };
        
        root.Children.Add(child);
        var categories = new[] { root };

        // Act
        var viewModel = ModelMappers.ToCategoryTreeViewModel("subject", categories);

        // Assert
        Assert.Single(viewModel.RootCategories);
        Assert.Equal("subject", viewModel.SchemaType);
        Assert.Single(viewModel.RootCategories[0].Children);
        Assert.Equal(root.Name, viewModel.RootCategories[0].Name);
        Assert.Equal(child.Name, viewModel.RootCategories[0].Children[0].Name);
    }

    [Fact]
    public void ToCategoryPathViewModel_BuildsBreadcrumbs()
    {
        // Arrange
        var root = new Category 
        { 
            Name = "Root", 
            Depth = 0,
            Path = "/Root"
        };
        var child = new Category 
        { 
            Name = "Child", 
            Parent = root, 
            Depth = 1,
            Path = "/Root/Child"
        };
        var grandChild = new Category 
        { 
            Name = "GrandChild", 
            Parent = child, 
            Depth = 2,
            Path = "/Root/Child/GrandChild"
        };

        // Act
        var viewModel = ModelMappers.ToCategoryPathViewModel(grandChild);

        // Assert
        Assert.Equal(3, viewModel.Breadcrumbs.Count);
        Assert.Equal("Root", viewModel.Breadcrumbs[0].Name);
        Assert.Equal("Child", viewModel.Breadcrumbs[1].Name);
        Assert.Equal("GrandChild", viewModel.Breadcrumbs[2].Name);
        Assert.Equal("/Root/Child/GrandChild", viewModel.Path);
    }

    [Fact]
    public void ToEntryWithCategoriesViewModel_GroupsBySchema()
    {
        // Arrange
        var scienceCategory = new Category 
        { 
            Name = "Science",
            SchemaType = "subject",
            Path = "/Science"
        };
        
        var physicsCategory = new Category 
        { 
            Name = "Physics",
            SchemaType = "subject",
            Path = "/Science/Physics",
            Parent = scienceCategory
        };
        
        var decadeCategory = new Category 
        { 
            Name = "2020s",
            SchemaType = "year",
            Path = "/2020s"
        };
        
        var yearCategory = new Category 
        { 
            Name = "2023",
            SchemaType = "year",
            Path = "/2020s/2023",
            Parent = decadeCategory
        };

        var entry = new Entry
        {
            Id = Ulid.NewUlid(),
            Title = "Test Entry",
            Categories = new List<CategoryEntry>
            {
                new() 
                { 
                    Category = physicsCategory
                },
                new() 
                { 
                    Category = yearCategory
                }
            }
        };

        // Act
        var viewModel = ModelMappers.ToEntryWithCategoriesViewModel(entry);

        // Assert
        Assert.Equal(2, viewModel.Categories.Count);
        Assert.True(viewModel.Categories.ContainsKey("subject"));
        Assert.True(viewModel.Categories.ContainsKey("year"));
        Assert.Equal("/Science/Physics", viewModel.Categories["subject"][0].Path);
        Assert.Equal("/2020s/2023", viewModel.Categories["year"][0].Path);
    }

    [Fact]
    public void ToCatalogWithCategoriesViewModel_IncludesCategoryTrees()
    {
        // Arrange
        var catalog = new Catalog
        {
            Id = Ulid.NewUlid(),
            Name = "Test Catalog",
            Categories = new List<Category>
            {
                new() 
                { 
                    Name = "Science",
                    SchemaType = "subject",
                    Children = new List<Category>
                    {
                        new() { Name = "Physics", SchemaType = "subject" }
                    }
                },
                new() 
                { 
                    Name = "2020s",
                    SchemaType = "decade",
                    Children = new List<Category>
                    {
                        new() { Name = "2023", SchemaType = "decade" }
                    }
                }
            }
        };

        // Act
        var viewModel = ModelMappers.ToCatalogWithCategoriesViewModel(catalog);

        // Assert
        Assert.Equal(2, viewModel.CategoryTrees.Count);
        Assert.True(viewModel.CategoryTrees.ContainsKey("subject"));
        Assert.True(viewModel.CategoryTrees.ContainsKey("decade"));
        Assert.Single(viewModel.CategoryTrees["subject"].RootCategories);
        Assert.Single(viewModel.CategoryTrees["decade"].RootCategories);
    }

    [Fact]
    public void ToSearchResultViewModel_IncludesCategoryPaths()
    {
        // Arrange
        var entry = new Entry
        {
            Id = Ulid.NewUlid(),
            Title = "Test Entry"
        };

        var categoryPaths = new[]
        {
            new CategoryPathViewModel 
            { 
                Name = "Physics",
                Path = "/Science/Physics",
                Breadcrumbs = new List<CategoryBreadcrumbViewModel>
                {
                    new() { Name = "Science", Depth = 0 },
                    new() { Name = "Physics", Depth = 1 }
                }
            }
        };

        // Act
        var viewModel = ModelMappers.ToSearchResultViewModel(entry, categoryPaths);

        // Assert
        Assert.Equal(entry.Title, viewModel.Entry.Title);
        Assert.Single(viewModel.CategoryPaths);
        Assert.Equal("/Science/Physics", viewModel.CategoryPaths[0].Path);
        Assert.Equal(2, viewModel.CategoryPaths[0].Breadcrumbs.Count);
    }

    [Fact]
    public void ToEntryWithCategoriesViewModel_HandlesStructuredData()
    {
        // Arrange
        var entry = new Entry
        {
            Id = Ulid.NewUlid(),
            Title = "Test Entry"
        };

        entry.CitationFields.AddField(new DataField
        {
            Key = "author",
            Value = "John Doe",
            DataType = "string",
            Schema = "bibtex",
            Order = 1
        });

        entry.MetadataFields.AddField(new DataField
        {
            Key = "rating",
            Value = "5",
            DataType = "number",
            Schema = "review",
            Order = 1
        });

        // Act
        var viewModel = ModelMappers.ToEntryWithCategoriesViewModel(entry);

        // Assert
        Assert.Single(viewModel.CitationFields);
        Assert.Single(viewModel.MetadataFields);
        Assert.Equal("author", viewModel.CitationFields[0].Key);
        Assert.Equal("rating", viewModel.MetadataFields[0].Key);
        Assert.Equal("bibtex", viewModel.CitationFields[0].Schema);
        Assert.Equal("review", viewModel.MetadataFields[0].Schema);
    }
} 