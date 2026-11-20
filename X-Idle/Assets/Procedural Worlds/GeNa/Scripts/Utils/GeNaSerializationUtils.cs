using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
#if !UNITY_6000_6_OR_NEWER
using System.Runtime.Serialization.Formatters.Binary;
#endif

/// <summary>
/// Version-specific serialization helpers for GeNa.Scripts.Core.
/// Lives outside the obfuscated Core DLL so Unity 6.6+ can avoid BinaryFormatter
/// while older editors keep the original binary format.
/// </summary>
public static class GeNaSerializationUtils
{
    private const string JsonObjectPrefix = "J1:";
    private const int KindJsonObject = 0;
    private const int KindPrimitive = 1;
    private const int KindByteArray = 2;

#if UNITY_6000_6_OR_NEWER
    private static Type s_binaryFormatterType;
    private static MethodInfo s_binaryFormatterDeserialize;
    private static bool s_binaryFormatterResolved;
#endif

    [Serializable]
    private class ObjectEnvelope
    {
        public int kind;
        public string typeName;
        public string json;
        public string primitive;
    }

    [Serializable]
    private class HeightMapSerializationData
    {
        public int widthX;
        public int depthZ;
        public byte[] metaData;
        public float[] heights;
    }

    public class HeightMapLoadResult
    {
        public int widthX;
        public int depthZ;
        public byte[] metaData;
        public float[,] heights;
    }

    /// <summary>
    /// Serializes an arbitrary object to a string.
    /// Unity 6.6+ stores a JsonUtility envelope; older Unity stores BinaryFormatter Base64.
    /// </summary>
    public static string SerializeToString(object value)
    {
        if (value == null)
        {
            return null;
        }

#if UNITY_6000_6_OR_NEWER
        try
        {
            ObjectEnvelope envelope = CreateObjectEnvelope(value);
            return JsonObjectPrefix + JsonUtility.ToJson(envelope);
        }
        catch (Exception)
        {
            Debug.LogWarning("Failed to serialize " + value.GetType());
            return null;
        }
#else
        try
        {
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, value);
                stream.Flush();
                return Convert.ToBase64String(stream.ToArray());
            }
        }
        catch (System.Runtime.Serialization.SerializationException)
        {
            Debug.LogWarning("Failed to serialize " + value.GetType().ToString());
            return null;
        }
#endif
    }

    /// <summary>
    /// Deserializes an object previously written by SerializeToString.
    /// Unity 6.6+ reads JSON envelopes and still loads legacy BinaryFormatter payloads.
    /// </summary>
    public static T DeserializeFromString<T>(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return default(T);
        }

        if (data.StartsWith(JsonObjectPrefix, StringComparison.Ordinal))
        {
            ObjectEnvelope envelope = JsonUtility.FromJson<ObjectEnvelope>(data.Substring(JsonObjectPrefix.Length));
            object value = CreateObjectFromEnvelope(envelope);
            if (value == null)
            {
                return default(T);
            }
            return (T)value;
        }

#if UNITY_6000_6_OR_NEWER
        byte[] bytes = Convert.FromBase64String(data);
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            return (T)LegacyBinaryFormatterDeserialize(stream);
        }
#else
        byte[] bytes = Convert.FromBase64String(data);
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }
#endif
    }

    /// <summary>
    /// Saves heightmap fields to disk.
    /// </summary>
    public static void SaveHeightMap(string fileName, int widthX, int depthZ, byte[] metaData, float[,] heights)
    {
#if UNITY_6000_6_OR_NEWER
        HeightMapSerializationData data = CreateHeightMapSerializationData(widthX, depthZ, metaData, heights);
        File.WriteAllText(fileName, JsonUtility.ToJson(data), new UTF8Encoding(false));
#else
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, widthX);
        formatter.Serialize(stream, depthZ);
        formatter.Serialize(stream, metaData);
        formatter.Serialize(stream, heights);
        stream.Close();
#endif
    }

    /// <summary>
    /// Loads heightmap fields from a file written by SaveHeightMap.
    /// Unity 6.6+ reads JSON and still loads legacy BinaryFormatter files.
    /// </summary>
    public static HeightMapLoadResult LoadHeightMapFromFile(string fileName)
    {
        byte[] bytes = File.ReadAllBytes(fileName);
        return LoadHeightMapFromBytes(bytes);
    }

    /// <summary>
    /// Loads heightmap fields from a serialized byte array.
    /// </summary>
    public static HeightMapLoadResult LoadHeightMapFromBytes(byte[] source)
    {
        if (source == null || source.Length == 0)
        {
            return null;
        }

        if (IsJsonPayload(source))
        {
            string json = Encoding.UTF8.GetString(source);
            if (!string.IsNullOrEmpty(json) && json[0] == '\uFEFF')
            {
                json = json.Substring(1);
            }
            return CreateHeightMapLoadResult(JsonUtility.FromJson<HeightMapSerializationData>(json));
        }

#if UNITY_6000_6_OR_NEWER
        using (MemoryStream stream = new MemoryStream(source))
        {
            return LoadHeightMapLegacy(stream);
        }
#else
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream(source);
        HeightMapLoadResult result = new HeightMapLoadResult();
        result.widthX = (int)formatter.Deserialize(stream);
        result.depthZ = (int)formatter.Deserialize(stream);
        result.metaData = (byte[])formatter.Deserialize(stream);
        result.heights = (float[,])formatter.Deserialize(stream);
        stream.Close();
        return result;
#endif
    }

    private static ObjectEnvelope CreateObjectEnvelope(object value)
    {
        Type type = value.GetType();
        ObjectEnvelope envelope = new ObjectEnvelope();
        envelope.typeName = type.AssemblyQualifiedName;

        byte[] byteArray = value as byte[];
        if (byteArray != null)
        {
            envelope.kind = KindByteArray;
            envelope.primitive = Convert.ToBase64String(byteArray);
            return envelope;
        }

        if (type.IsPrimitive || type.IsEnum || value is string || value is decimal || value is DateTime)
        {
            envelope.kind = KindPrimitive;
            if (value is DateTime)
            {
                envelope.primitive = ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            }
            else if (type.IsEnum)
            {
                envelope.primitive = Convert.ToInt32(value).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                envelope.primitive = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            return envelope;
        }

        envelope.kind = KindJsonObject;
        envelope.json = JsonUtility.ToJson(value);
        return envelope;
    }

    private static object CreateObjectFromEnvelope(ObjectEnvelope envelope)
    {
        if (envelope == null || string.IsNullOrEmpty(envelope.typeName))
        {
            return null;
        }

        Type type = Type.GetType(envelope.typeName);
        if (type == null)
        {
            return null;
        }

        if (envelope.kind == KindByteArray)
        {
            if (string.IsNullOrEmpty(envelope.primitive))
            {
                return new byte[0];
            }
            return Convert.FromBase64String(envelope.primitive);
        }

        if (envelope.kind == KindPrimitive)
        {
            if (type == typeof(string))
            {
                return envelope.primitive;
            }
            if (type == typeof(DateTime))
            {
                return DateTime.Parse(envelope.primitive, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            if (type.IsEnum)
            {
                return Enum.ToObject(type, Convert.ToInt32(envelope.primitive, CultureInfo.InvariantCulture));
            }
            return Convert.ChangeType(envelope.primitive, type, CultureInfo.InvariantCulture);
        }

        return JsonUtility.FromJson(envelope.json, type);
    }

    private static HeightMapSerializationData CreateHeightMapSerializationData(int widthX, int depthZ, byte[] metaData, float[,] heights)
    {
        HeightMapSerializationData data = new HeightMapSerializationData();
        data.widthX = widthX;
        data.depthZ = depthZ;
        data.metaData = metaData;
        if (data.metaData == null)
        {
            data.metaData = new byte[0];
        }

        int count = widthX * depthZ;
        data.heights = new float[count];
        if (heights != null && count > 0)
        {
            Buffer.BlockCopy(heights, 0, data.heights, 0, count * sizeof(float));
        }
        return data;
    }

    private static HeightMapLoadResult CreateHeightMapLoadResult(HeightMapSerializationData data)
    {
        if (data == null)
        {
            return null;
        }

        HeightMapLoadResult result = new HeightMapLoadResult();
        result.widthX = data.widthX;
        result.depthZ = data.depthZ;
        result.metaData = data.metaData;
        if (result.metaData == null)
        {
            result.metaData = new byte[0];
        }

        result.heights = new float[result.widthX, result.depthZ];
        if (data.heights != null && data.heights.Length > 0)
        {
            int bytesToCopy = Math.Min(data.heights.Length, result.widthX * result.depthZ) * sizeof(float);
            Buffer.BlockCopy(data.heights, 0, result.heights, 0, bytesToCopy);
        }
        return result;
    }

#if UNITY_6000_6_OR_NEWER
    private static HeightMapLoadResult LoadHeightMapLegacy(Stream stream)
    {
        HeightMapLoadResult result = new HeightMapLoadResult();
        result.widthX = (int)LegacyBinaryFormatterDeserialize(stream);
        result.depthZ = (int)LegacyBinaryFormatterDeserialize(stream);
        result.metaData = (byte[])LegacyBinaryFormatterDeserialize(stream);
        result.heights = (float[,])LegacyBinaryFormatterDeserialize(stream);
        return result;
    }

    /// <summary>
    /// Loads pre-6.6 BinaryFormatter payloads without a compile-time BinaryFormatter reference,
    /// so UAC0023 stays silent in Unity 6.6 while existing files/assets still open.
    /// </summary>
    private static object LegacyBinaryFormatterDeserialize(Stream stream)
    {
        ResolveBinaryFormatter();
        if (s_binaryFormatterType == null || s_binaryFormatterDeserialize == null)
        {
            throw new InvalidOperationException("BinaryFormatter is not available to load legacy GeNa data.");
        }

        object formatter = Activator.CreateInstance(s_binaryFormatterType);
        try
        {
            return s_binaryFormatterDeserialize.Invoke(formatter, new object[] { stream });
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
            throw;
        }
    }

    private static void ResolveBinaryFormatter()
    {
        if (s_binaryFormatterResolved)
        {
            return;
        }

        s_binaryFormatterResolved = true;
        s_binaryFormatterType = Type.GetType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter");
        if (s_binaryFormatterType == null)
        {
            s_binaryFormatterType = Type.GetType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter, mscorlib");
        }
        if (s_binaryFormatterType != null)
        {
            s_binaryFormatterDeserialize = s_binaryFormatterType.GetMethod("Deserialize", new Type[] { typeof(Stream) });
        }
    }
#endif

    private static bool IsJsonPayload(byte[] bytes)
    {
        int index = 0;
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            index = 3;
        }

        while (index < bytes.Length)
        {
            byte value = bytes[index];
            if (value == (byte)' ' || value == (byte)'\t' || value == (byte)'\r' || value == (byte)'\n')
            {
                index++;
                continue;
            }

            return value == (byte)'{';
        }

        return false;
    }
}
