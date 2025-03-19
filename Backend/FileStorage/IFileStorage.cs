namespace FileStorage;

public interface IFileStorage
{
    Task WriteDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId, Stream data);
    Stream GetDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId);

    Task WriteAnalyzerScript(Guid courseId, Guid assignmentId, Guid analyzerId, Stream data);
    Task<string> GetAnalyzerScript(Guid courseId, Guid assignmentId, Guid analyzerId);

    Task WriteAnalysisField(Guid courseId, Guid assignmentId, Guid analyzerId, Guid analysisId, Guid entryId, Guid analysisFieldId, Stream data);
    Stream GetAnalysisField(Guid courseId, Guid assignmentId, Guid analyzerId, Guid analysisId, Guid entryId, Guid analysisFieldId);

    bool DeleteAll();
    bool DeleteCourse(Guid courseId);
    bool DeleteAssignment(Guid courseId, Guid assignmentId);
    bool DeleteDelivery(Guid courseId, Guid assignmentId, Guid deliveryId);
    bool DeleteDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId);
    bool DeleteAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId);
    bool DeleteAnalysis(Guid courseId, Guid assignmentId, Guid analyzerId, Guid analysisId);
}
