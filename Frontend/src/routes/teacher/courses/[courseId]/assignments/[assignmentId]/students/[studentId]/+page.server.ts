import { getCourse, getAssignment, getAssignmentFields, getStudentFeedback, getStudent, getStudentDelivery, getStudentAnalyses } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; studentId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveryResponse,
        feedbackResponse,
        studentResponse,
        studentAnalysesResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getStudentDelivery(params.studentId, params.assignmentId),
        getStudentFeedback(params.studentId, params.assignmentId),
        getStudent(params.studentId),
        getStudentAnalyses(params.studentId, params.assignmentId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        delivery: deliveryResponse.status == 204 ? null : deliveryResponse?.data,
        feedback: feedbackResponse.status == 204 ? null : feedbackResponse?.data,
        student: studentResponse.data,
        studentAnalyses: studentAnalysesResponse.data
    }
};