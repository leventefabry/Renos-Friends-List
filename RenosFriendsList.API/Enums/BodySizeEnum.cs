using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RenosFriendsList.API.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodySizeEnum
    {
        [EnumMember(Value = "Small")]
        SmallDog = 1,

        [EnumMember(Value = "Medium")]
        Medium = 2,

        [EnumMember(Value = "Large")]
        Large = 3
    }
}
