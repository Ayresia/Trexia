namespace Valorant_BOT
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ValorantEvent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("regions")]
        public List<Region> Regions { get; set; }
    }

    public partial class Region
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("maintenances")]
        public List<Incident> Maintenances { get; set; }

        [JsonProperty("incidents")]
        public List<Incident> Incidents { get; set; }
    }

    public partial class Incident
    {
        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("platforms")]
        public List<Platform> Platforms { get; set; }

        [JsonProperty("maintenance_status")]
        public string MaintenanceStatus { get; set; }

        [JsonProperty("incident_severity")]
        public string IncidentSeverity { get; set; }

        [JsonProperty("updates")]
        public List<Update> Updates { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public partial class Update
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public enum Description { GunSkinDisables, MaintenanceNotification };

    public enum Platform { Windows };

    public partial class ValorantEvent
    {
        public static List<ValorantEvent> FromJson(string json) => JsonConvert.DeserializeObject<List<ValorantEvent>>(json, Valorant_BOT.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<ValorantEvent> self) => JsonConvert.SerializeObject(self, Valorant_BOT.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                DescriptionConverter.Singleton,
                PlatformConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DescriptionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Description) || t == typeof(Description?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Gun Skin Disables":
                    return Description.GunSkinDisables;
                case "Maintenance Notification":
                    return Description.MaintenanceNotification;
            }
            throw new Exception("Cannot unmarshal type Description");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Description)untypedValue;
            switch (value)
            {
                case Description.GunSkinDisables:
                    serializer.Serialize(writer, "Gun Skin Disables");
                    return;
                case Description.MaintenanceNotification:
                    serializer.Serialize(writer, "Maintenance Notification");
                    return;
            }
            throw new Exception("Cannot marshal type Description");
        }

        public static readonly DescriptionConverter Singleton = new DescriptionConverter();
    }

    internal class PlatformConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Platform) || t == typeof(Platform?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "windows")
            {
                return Platform.Windows;
            }
            throw new Exception("Cannot unmarshal type Platform");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Platform)untypedValue;
            if (value == Platform.Windows)
            {
                serializer.Serialize(writer, "windows");
                return;
            }
            throw new Exception("Cannot marshal type Platform");
        }

        public static readonly PlatformConverter Singleton = new PlatformConverter();
    }
}
