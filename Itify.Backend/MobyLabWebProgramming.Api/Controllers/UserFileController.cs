using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using MobyLabWebProgramming.Infrastructure.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.Authorization;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Api.Controllers;

/// <summary>
/// This is a controller example to show who to work with files and form data.
/// Inject the required services through the constructor.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class UserFileController(ILogger<UserFileController> logger, IUserService userService, IUserFileService userFileService) : AuthorizedController(logger, userService)
{
    private const long MaxFileSize = 128 * 1024 * 1024; // Set the maximum size for file requests to 128MB.

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on page of user files.
    /// Generally, if you need to get multiple values from the database use pagination if there are many entries.
    /// It will improve performance and reduce resource consumption for both client and server.
    /// </summary>
    [Authorize]
    [HttpGet] // This attribute will make the controller respond to a HTTP GET request on the route /api/UserFile/GetPage.
    public async Task<ActionResult<RequestResponse<PagedResponse<UserFileRecord>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await userFileService.GetUserFiles(pagination)) :
            ErrorMessageResult<PagedResponse<UserFileRecord>>(currentUser.Error);
    }

    /// <summary>
    /// This method adds a user file from a multipart/form-data request.
    /// </summary>
    [Authorize]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)] // Sets the maximum size limit for the form body to override the default value
    [RequestSizeLimit(MaxFileSize)] // Sets the maximum size limit for the entire request to override the default value
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/UserFile/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromForm] UserFileAddRecord form) // The FromForm attribute will bind each field from the form request to the properties of the UserFileAddRecord parameter.
                                                                                         // For files the property should be IFormFile or IFormFileCollection.
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await userFileService.SaveFile(form, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method downloads a user file.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}")]
    [Produces(MediaTypeNames.Application.Octet, MediaTypeNames.Application.Json)] // Sets the possible response MIME types because on success a binary file is send while on error a error JSON is send.
    [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)] // On success a Stream should be send. 
    public async Task<ActionResult<RequestResponse<FileRecord>>> Download([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
        {
            return ErrorMessageResult<FileRecord>(currentUser.Error);
        }

        var file = await userFileService.GetFileDownload(id);

        return FromServiceResponse(file);
    }
}
