import { getCourse, getAssignment, getAssignmentFields, getStudentDelivery, getStudentFeedback } from "src/api.server";
import { studentId } from "src/store";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: {params: { courseId: string; assignmentId: string; }}) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveryResponse,
        feedbackResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getStudentDelivery(studentId, params.assignmentId),
        getStudentFeedback(studentId, params.assignmentId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        delivery: deliveryResponse.status == 204 ? null : deliveryResponse?.data,
        feedback: feedbackResponse.status == 204 ? null : feedbackResponse?.data,
    }
};