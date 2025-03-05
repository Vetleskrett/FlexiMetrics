using Database.Models;

namespace Api.Common;

public class FileResponse
{
    public required Stream Stream { get; init; }
    public required FileMetadata Metadata { get; init; }
}
