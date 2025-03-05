import { getCourse, getAssignment, getAssignmentFields, getTeamDelivery, getTeamFeedback, getTeam } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; teamId: string; } }) => {
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
        getTeamDelivery(params.teamId, params.assignmentId),
        getTeamFeedback(params.teamId, params.assignmentId),
        getTeam(params.teamId),
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        delivery: deliveryResponse.status == 204 ? null : deliveryResponse?.data,
        feedback: feedbackResponse.status == 204 ? null : feedbackResponse?.data,
        team: teamResponse.data,
    }
};