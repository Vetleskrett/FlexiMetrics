import { getCourse, getAssignment, getAssignmentFields, getStudentFeedback, getStudent, getStudentDelivery } from "src/api";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; studentId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveryResponse,
        feedbackResponse,
        studentResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getStudentDelivery(params.studentId, params.assignmentId).catch(e => null),
        getStudentFeedback(params.studentId, params.assignmentId).catch(e => null),
        getStudent(params.studentId),
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        delivery: deliveryResponse?.data,
        feedback: feedbackResponse?.data,
        student: studentResponse.data,
    }
};