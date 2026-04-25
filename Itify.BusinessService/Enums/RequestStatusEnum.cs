using System.Text.Json.Serialization;

namespace Itify.BusinessService.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RequestStatusEnum { Pending, Approved, Rejected }
