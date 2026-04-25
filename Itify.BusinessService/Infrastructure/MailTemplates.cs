namespace Itify.BusinessService.Infrastructure;

public static class MailTemplates
{
    public static string DeviceRequestCreated(string employeeName, string categoryName, string reason, string frontendUrl) => $@"
<p>A new device request has been submitted.</p>
<p><strong>Employee:</strong> {employeeName}</p>
<p><strong>Category:</strong> {categoryName}</p>
<p><strong>Reason:</strong> {reason}</p>
<p><a href=""{frontendUrl}/device-requests"">View all requests</a></p>";

    public static string DeviceRequestStatusUpdated(string employeeName, string categoryName, string status, string frontendUrl) => $@"
<p>Dear {employeeName},</p>
<p>Your device request for <strong>{categoryName}</strong> has been <strong>{status}</strong>.</p>
<p><a href=""{frontendUrl}/device-requests"">View your requests</a></p>";

    public static string DeviceAssigned(string employeeName, string deviceName, string serialNumber, string frontendUrl) => $@"
<p>Dear {employeeName},</p>
<p>A device has been assigned to you.</p>
<p><strong>Device:</strong> {deviceName} ({serialNumber})</p>
<p><a href=""{frontendUrl}/devices"">View your devices</a></p>";

    public static string TicketCreated(string employeeName, string description, string type, string frontendUrl) => $@"
<p>A new ticket has been submitted.</p>
<p><strong>Employee:</strong> {employeeName}</p>
<p><strong>Type:</strong> {type}</p>
<p><strong>Description:</strong> {description}</p>
<p><a href=""{frontendUrl}/tickets"">View all tickets</a></p>";

    public static string TicketStatusUpdated(string employeeName, string description, string status, string frontendUrl) => $@"
<p>Dear {employeeName},</p>
<p>Your ticket <strong>""{description}""</strong> has been updated to <strong>{status}</strong>.</p>
<p><a href=""{frontendUrl}/tickets"">View your tickets</a></p>";
}
