namespace FileStorage;

public interface IFileStorage
{
    Task WriteDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId, Stream data);
    Stream ReadDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId);

    Task WriteAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId, Stream data);
    Stream ReadAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId);

    bool DeleteAll();
    bool DeleteCourse(Guid courseId);
    bool DeleteAssignment(Guid courseId, Guid assignmentId);
    bool DeleteDelivery(Guid courseId, Guid assignmentId, Guid deliveryId);
    bool DeleteDeliveryField(Guid courseId, Guid assignmentId, Guid deliveryId, Guid deliveryFieldId);
    bool DeleteAnalyzer(Guid courseId, Guid assignmentId, Guid analyzerId);
}
