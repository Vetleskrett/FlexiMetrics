namespace FileStorage;

public class LocalFileStorage : IFileStorage
{
    private static readonly string AppDataPath = Path.Combine
    (
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "fleximetrics"
    );

    public async Task WriteDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId, Stream data)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assignmentId, deliveryId);
        Directory.CreateDirectory(dirPath);

        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await data.CopyToAsync(fileStream);
    }

    public Stream GetDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assignmentId, deliveryId);
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        return File.OpenRead(filePath);
    }

    public async Task WriteAnalyzerScript(Guid courseId, Guid assignmentId, Guid analyzerId, Stream data)
    {
        var dirPath = GetAnalyzerDirectoryPath(courseId, assignmentId);
        Directory.CreateDirectory(dirPath);

        var filePath = Path.Combine(dirPath, analyzerId.ToString());
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await data.CopyToAsync(fileStream);
    }

    public Stream GetAnalyzerScript(Guid courseId, Guid assignmentId, Guid analyzerId)
    {
        var dirPath = GetAnalyzerDirectoryPath(courseId, assignmentId);
        var filePath = Path.Combine(dirPath, analyzerId.ToString());
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

    public bool DeleteAssignment(Guid courseId, Guid assignmentId)
    {
        var dirPath = GetAssignmentDirectoryPath(courseId, assignmentId);
        var exists = Directory.Exists(dirPath);
        if (exists)
        {
            Directory.Delete(dirPath, true);
        }
        return exists;
    }

    public bool DeleteDelivery(Guid courseId, Guid assignmentId, Guid deliveryId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assignmentId, deliveryId);
        var exists = Directory.Exists(dirPath);
        if (exists)
        {
            Directory.Delete(dirPath, true);
        }
        return exists;
    }

    public bool DeleteDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId)
    {
        var dirPath = GetDeliveryDirectoryPath(courseId, assignmentId, deliveryId);
        var filePath = Path.Combine(dirPath, deliveryFieldId.ToString());
        var exists = File.Exists(filePath);
        if (exists)
        {
            File.Delete(filePath);
        }
        return exists;
    }

    public bool DeleteAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId)
    {
        var dirPath = GetAnalyzerDirectoryPath(courseId, assignmentId);
        var filePath = Path.Combine(dirPath, analyzerId.ToString());
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

    private string GetAssignmentDirectoryPath(Guid courseId, Guid assignmentId)
    {
        return Path.Combine
        (
            AppDataPath,
            "courses",
            courseId.ToString(),
            "assignments",
            assignmentId.ToString()
        );
    }

    private string GetDeliveryDirectoryPath(Guid courseId, Guid assignmentId, Guid deliveryId)
    {
        return Path.Combine
        (
            AppDataPath,
            "courses",
            courseId.ToString(),
            "assignments",
            assignmentId.ToString(),
            "deliveries",
            deliveryId.ToString()
        );
    }

    private string GetAnalyzerDirectoryPath(Guid courseId, Guid assignmentId)
    {
        return Path.Combine
        (
            AppDataPath,
            "courses",
            courseId.ToString(),
            "assignments",
            assignmentId.ToString(),
            "analyzers"
        );
    }
}
