using Database.Models;

namespace Api.Deliveries.Contracts;

public class FileResponse
{
    public required Stream Stream { get; init; }
    public required FileMetadata Metadata { get; init; }
}
