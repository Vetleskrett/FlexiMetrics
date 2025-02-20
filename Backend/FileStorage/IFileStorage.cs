namespace FileStorage;

public interface IFileStorage
{
    Task WriteDeliveryFile(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId, Stream data);
    Stream ReadDeliveryFile(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId);
    bool DeleteDeliveryFile(Guid courseId, Guid assigmentId, Guid deliveryId, Guid deliveryFieldId);
}
