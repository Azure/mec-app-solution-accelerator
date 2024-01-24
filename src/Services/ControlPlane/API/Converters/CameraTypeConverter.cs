using System.Text.Json;
using System.Text.Json.Serialization;
using ControlPlane.API.Models;

public class CameraTypeConverter : JsonConverter<CameraType>
{
    public override CameraType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value == null)
        {
            return CameraType.Unknown;
        }

        try
        {
            return value switch
            {
                "5G" => CameraType.FiveG,
                "4G" => CameraType.FourG,
                _ => Enum.Parse<CameraType>(value, true)
            };
        }
        catch
        {
            return CameraType.Unknown;
        }
    }

    public override void Write(Utf8JsonWriter writer, CameraType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            CameraType.FiveG => "5G",
            CameraType.FourG => "4G",
            _ => value.ToString()
        };
        writer.WriteStringValue(stringValue);
    }
}