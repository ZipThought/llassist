using llassist.Common.Mappers;
using llassist.Common.Models.Library;
using Xunit;
using Assert = Xunit.Assert;

namespace llassist.Tests;

public class LibraryModelTests
{
    [Fact]
    public void Catalog_InitializesCollections()
    {
        // Arrange & Act
        var catalog = new Catalog();

        // Assert
        Assert.NotNull(catalog.Entries);
        Assert.NotNull(catalog.Labels);
        Assert.NotNull(catalog.Categories);
        Assert.Empty(catalog.Entries);
        Assert.Empty(catalog.Labels);
        Assert.Empty(catalog.Categories);
    }

    [Fact]
    public void Category_CalculatesPathAndDepth()
    {
        // Arrange
        var root = new Category { Name = "Science" };
        var physics = new Category { Name = "Physics", Parent = root };
        var quantum = new Category { Name = "Quantum", Parent = physics };

        // Act
        var path = ModelMappers.BuildCategoryPath(quantum);
        var depth = ModelMappers.CalculateCategoryDepth(quantum);

        // Assert
        Assert.Equal("/Science/Physics/Quantum", path);
        Assert.Equal(2, depth);
    }

    [Fact]
    public void Entry_HandlesMetadata()
    {
        // Arrange
        var entry = new Entry
        {
            Title = "Test Entry",
            Metadata = """{"key": "value", "number": 42}"""
        };

        // Act
        var viewModel = ModelMappers.ToEntryViewModel(entry);

        // Assert
        Assert.NotNull(viewModel.Metadata);
        Assert.Equal(2, viewModel.Metadata.Count);
        Assert.True(viewModel.Metadata.ContainsKey("key"));
        Assert.True(viewModel.Metadata.ContainsKey("number"));
    }
} 