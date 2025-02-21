namespace FileStorage;

public interface IFileStorage
{
    Task WriteDeliveryField(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId, Stream data);
    Stream ReadDeliveryField(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId);

    bool DeleteCourse(Guid courseId);
    bool DeleteAssignment(Guid courseId, Guid assigmentId);
    bool DeleteDelivery(Guid courseId, Guid assigmentId, Guid deliveryId);
    bool DeleteDeliveryField(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId);
}
