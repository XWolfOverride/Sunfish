using System;

namespace Json.Net
{
    /// <summary>
    /// Converter interface
    /// </summary>
    public interface IJsonConverter
    {
        Type GetConvertingType();
        string Serializer(object obj);
        object Deserializer(string txt);
    }
}
