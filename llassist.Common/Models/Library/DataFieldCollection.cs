using System.Collections.ObjectModel;

namespace llassist.Common.Models.Library;

public class DataFieldCollection
{
    private List<DataField> _fields = new();
    
    public IReadOnlyCollection<DataField> Fields => _fields.AsReadOnly();
    
    public DataFieldCollection(List<DataField>? fields = null)
    {
        _fields = fields ?? new List<DataField>();
    }
    
    public void AddField(DataField field)
    {
        if (_fields.Any(f => f.Key == field.Key && f.Schema == field.Schema))
        {
            throw new InvalidOperationException($"Field with key '{field.Key}' already exists in schema '{field.Schema}'");
        }
        _fields.Add(field);
    }
    
    public void RemoveField(string key, string schema)
    {
        var field = _fields.FirstOrDefault(f => f.Key == key && f.Schema == schema);
        if (field != null)
        {
            _fields.Remove(field);
        }
    }
    
    public DataField? GetField(string key, string schema)
    {
        return _fields.FirstOrDefault(f => f.Key == key && f.Schema == schema);
    }
} 