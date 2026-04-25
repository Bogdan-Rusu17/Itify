using System.Text.Json.Serialization;

namespace Itify.BusinessService.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TicketTypeEnum { RepairRequest, GeneralIssue, SoftwareIssue }
