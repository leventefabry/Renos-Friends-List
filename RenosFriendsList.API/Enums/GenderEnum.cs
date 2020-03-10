using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RenosFriendsList.API.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GenderEnum
    {
        [EnumMember(Value = "Male")]
        Male = 1,

        [EnumMember(Value = "Female")]
        Female = 2
    }
}
