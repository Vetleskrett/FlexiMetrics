import { getCourse, getAssignment, getAssignmentFields, getTeamDelivery, getTeamFeedback, getTeam } from "src/api";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; teamId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveryResponse,
        feedbackResponse,
        teamResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getTeamDelivery(params.teamId, params.assignmentId).catch(e => null),
        getTeamFeedback(params.teamId, params.assignmentId).catch(e => null),
        getTeam(params.teamId),
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        delivery: deliveryResponse?.data,
        feedback: feedbackResponse?.data,
        team: teamResponse.data,
    }
};