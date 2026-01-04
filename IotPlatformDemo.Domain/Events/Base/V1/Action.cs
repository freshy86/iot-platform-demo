using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IotPlatformDemo.Domain.Events.Base.V1;

[JsonConverter(typeof(StringEnumConverter))]
public enum Action {
    Create,
    Update,
    Delete
}