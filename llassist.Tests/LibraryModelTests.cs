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
    public void Entry_InitializesCollections()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.NotNull(entry.CitationFields);
        Assert.NotNull(entry.MetadataFields);
        Assert.NotNull(entry.Resources);
        Assert.NotNull(entry.Labels);
        Assert.NotNull(entry.Categories);
        Assert.Empty(entry.Resources);
        Assert.Empty(entry.Labels);
        Assert.Empty(entry.Categories);
    }

    [Fact]
    public void Entry_HandlesCitationFields()
    {
        // Arrange
        var entry = new Entry { Title = "Test Entry" };
        
        // Act
        entry.CitationFields.AddField(new DataField
        {
            Key = "author",
            Value = "John Doe",
            DataType = "string",
            Schema = "bibtex",
            Order = 1,
            Required = true
        });

        // Assert
        var field = entry.CitationFields.GetField("author", "bibtex");
        Assert.NotNull(field);
        Assert.Equal("John Doe", field.Value);
        Assert.Equal("string", field.DataType);
        Assert.True(field.Required);
    }

    [Fact]
    public void Entry_HandlesMetadataFields()
    {
        // Arrange
        var entry = new Entry { Title = "Test Entry" };
        
        // Act
        entry.MetadataFields.AddField(new DataField
        {
            Key = "rating",
            Value = "5",
            DataType = "number",
            Schema = "review",
            Order = 1
        });

        // Assert
        var field = entry.MetadataFields.GetField("rating", "review");
        Assert.NotNull(field);
        Assert.Equal("5", field.Value);
        Assert.Equal("number", field.DataType);
        Assert.Equal("review", field.Schema);
    }

    [Fact]
    public void DataFieldCollection_PreventsDuplicateKeys()
    {
        // Arrange
        var entry = new Entry();
        entry.CitationFields.AddField(new DataField
        {
            Key = "author",
            Value = "John Doe",
            Schema = "bibtex"
        });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            entry.CitationFields.AddField(new DataField
            {
                Key = "author",
                Value = "Jane Doe",
                Schema = "bibtex"
            })
        );
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
} 