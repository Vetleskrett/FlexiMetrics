namespace FileStorage;

public class LocalFileStorage : IFileStorage
{
    private static readonly string AppDataPath = Path.Combine
    (
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "fleximetrics"
    );

    private string GetDeliveryDirectoryPath(Guid courseId, Guid assigmentId, Guid deliveryId)
    {
        return Path.Combine
        (
            AppDataPath,
            "courses",
            courseId.ToString(),
            "assignments",
            assigmentId.ToString(),
            "deliveries",
            deliveryId.ToString()
        );
    }

    public async Task WriteDeliveryFile(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId, Stream data)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        Directory.CreateDirectory(dirPath);
        
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await data.CopyToAsync(fileStream);
    }

    public Stream ReadDeliveryFile(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        return File.OpenRead(filePath);
    }

    public bool DeleteDeliveryFile(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        var exists = File.Exists(filePath);
        File.Delete(filePath);
        return exists;
    }
}
