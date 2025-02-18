import { getCourse, getAssignment, getAssignmentFields, getStudentDelivery, getStudentFeedback } from "src/api";
import { studentId } from "src/store";

export const load = async ({ params }: {params: { courseId: string; assignmentId: string; }}) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveryResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getStudentDelivery(studentId, params.assignmentId),
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        delivery: deliveryResponse.status == 204 ? null : deliveryResponse?.data,
    }
};