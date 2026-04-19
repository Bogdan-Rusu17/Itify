namespace Itify.Services.Constants;

public static class MailTemplates
{
    public static string UserAddTemplate(string name) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>Welcome to Itify</title>
</head>
<body>
    <p>Dear {name},</p>
    <p>Welcome to Itify! Your account has been created.</p>
</body>
</html>";

    public static string DeviceRequestCreatedTemplate(string employeeName, string categoryName, string reason, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>New Device Request</title>
</head>
<body>
    <p>A new device request has been submitted.</p>
    <p><strong>Employee:</strong> {employeeName}</p>
    <p><strong>Category:</strong> {categoryName}</p>
    <p><strong>Reason:</strong> {reason}</p>
    <p><a href=""{frontendUrl}/device-requests"">View all requests</a></p>
</body>
</html>";

    public static string DeviceRequestStatusUpdatedTemplate(string employeeName, string categoryName, string status, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>Device Request Updated</title>
</head>
<body>
    <p>Dear {employeeName},</p>
    <p>Your device request for <strong>{categoryName}</strong> has been <strong>{status}</strong>.</p>
    <p><a href=""{frontendUrl}/device-requests"">View your requests</a></p>
</body>
</html>";

    public static string TicketCreatedTemplate(string employeeName, string description, string type, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>New Ticket</title>
</head>
<body>
    <p>A new ticket has been submitted.</p>
    <p><strong>Employee:</strong> {employeeName}</p>
    <p><strong>Type:</strong> {type}</p>
    <p><strong>Description:</strong> {description}</p>
    <p><a href=""{frontendUrl}/tickets"">View all tickets</a></p>
</body>
</html>";

    public static string TicketStatusUpdatedTemplate(string employeeName, string description, string status, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>Ticket Updated</title>
</head>
<body>
    <p>Dear {employeeName},</p>
    <p>Your ticket <strong>""{description}""</strong> has been updated to <strong>{status}</strong>.</p>
    <p><a href=""{frontendUrl}/tickets"">View your tickets</a></p>
</body>
</html>";

    public static string DeviceAssignedTemplate(string employeeName, string deviceName, string serialNumber, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>Device Assigned</title>
</head>
<body>
    <p>Dear {employeeName},</p>
    <p>A device has been assigned to you.</p>
    <p><strong>Device:</strong> {deviceName}</p>
    <p><strong>Serial number:</strong> {serialNumber}</p>
    <p><a href=""{frontendUrl}/devices"">View your devices</a></p>
</body>
</html>";
}
