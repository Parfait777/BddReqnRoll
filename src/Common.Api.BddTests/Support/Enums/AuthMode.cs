using System.Runtime.Serialization;

namespace Common.Api.BddTests.Support.Enums
{
    /// <summary>
    /// LIsts the supported authentication modes.
    /// </summary>
    public enum AuthMode
    {
        [EnumMember(Value = "Bearer")]
        Bearer,

        [EnumMember(Value = "ApiKey")]
        ApiKey
    }
}
