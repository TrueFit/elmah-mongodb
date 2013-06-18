#region usings

using System;
using System.Collections.Specialized;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

#endregion

namespace TruFit.Web.Elmah.MongoDB
{
  public class NameValueCollectionSerializer : BsonBaseSerializer
  {
    private static readonly NameValueCollectionSerializer instance = new NameValueCollectionSerializer();

    public static NameValueCollectionSerializer Instance
    {
      get { return instance; }
    }

    public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
    {
      return Deserialize(bsonReader, nominalType, options);
    }

    public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
    {
      var bsonType = bsonReader.GetCurrentBsonType();
      if (bsonType == BsonType.Null)
      {
        bsonReader.ReadNull();
        return null;
      }

      var serializer = new StringSerializer();
      var nvc = new NameValueCollection();

      bsonReader.ReadStartArray();
      while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
      {
        bsonReader.ReadStartArray();
        var key = (string) serializer.Deserialize(bsonReader, typeof (string), options);
        var val = (string) serializer.Deserialize(bsonReader, typeof (string), options);
        bsonReader.ReadEndArray();
        nvc.Add(key, val);
      }
      bsonReader.ReadEndArray();

      return nvc;
    }

    public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
    {
      if (value == null)
      {
        bsonWriter.WriteNull();
        return;
      }

      var serializer = new StringSerializer();
      var nvc = (NameValueCollection) value;

      bsonWriter.WriteStartArray();
      foreach (var key in nvc.AllKeys)
      {
        foreach (var val in nvc.GetValues(key))
        {
          bsonWriter.WriteStartArray();
          serializer.Serialize(bsonWriter, typeof (string), key, options);
          serializer.Serialize(bsonWriter, typeof (string), val, options);
          bsonWriter.WriteEndArray();
        }
      }
      bsonWriter.WriteEndArray();
    }
  }
}