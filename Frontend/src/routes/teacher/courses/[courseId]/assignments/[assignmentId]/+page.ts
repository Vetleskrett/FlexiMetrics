import { getTeacherCourse, getAssignment, getAssignmentFields, getDeliveries } from "src/api";
import { teacherId } from "src/store";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveriesResponse,
    ] = await Promise.all([
        getTeacherCourse(teacherId, params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getDeliveries(params.assignmentId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        deliveries: deliveriesResponse.data
    }
};