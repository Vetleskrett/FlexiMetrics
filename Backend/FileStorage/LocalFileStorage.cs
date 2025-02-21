namespace FileStorage;

public class LocalFileStorage : IFileStorage
{
    private static readonly string AppDataPath = Path.Combine
    (
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "fleximetrics"
    );

    public async Task WriteDeliveryField(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId, Stream data)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        Directory.CreateDirectory(dirPath);
        
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await data.CopyToAsync(fileStream);
    }

    public Stream ReadDeliveryField(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        return File.OpenRead(filePath);
    }

    public bool DeleteAll()
    {
        var exists = Directory.Exists(AppDataPath);
        if (exists)
        {
            Directory.Delete(AppDataPath, true);
        }
        return exists;
    }

    public bool DeleteCourse(Guid courseId)
    {
        var dirPath = GetCourseDirectoryPath(courseId);
        var exists = Directory.Exists(dirPath);
        if (exists)
        {
            Directory.Delete(dirPath, true);
        }
        return exists;
    }

    public bool DeleteAssignment(Guid courseId, Guid assigmentId)
    {
        var dirPath = GetAssignmentDirectoryPath(courseId, assigmentId);
        var exists = Directory.Exists(dirPath);
        if (exists)
        {
            Directory.Delete(dirPath, true);
        }
        return exists;
    }

    public bool DeleteDelivery(Guid courseId, Guid assigmentId, Guid deliveryId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        var exists = Directory.Exists(dirPath);
        if (exists)
        {
            Directory.Delete(dirPath, true);
        }
        return exists;
    }

    public bool DeleteDeliveryField(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assigmentId, deliveryId);
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        var exists = File.Exists(filePath);
        if (exists)
        {
            File.Delete(filePath);
        }
        return exists;
    }

    private string GetCourseDirectoryPath(Guid courseId)
    {
        return Path.Combine
        (
            AppDataPath,
            "courses",
            courseId.ToString()
        );
    }

    private string GetAssignmentDirectoryPath(Guid courseId, Guid assigmentId)
    {
        return Path.Combine
        (
            AppDataPath,
            "courses",
            courseId.ToString(),
            "assignments",
            assigmentId.ToString()
        );
    }

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
}
