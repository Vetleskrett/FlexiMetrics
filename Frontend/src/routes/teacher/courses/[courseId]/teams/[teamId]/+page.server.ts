import { getCourse, getTeam, getTeacherAssignments, getTeamAssignmentsProgress } from "src/api.server";

export const load = async ({ params }: {params: {courseId: string; teamId: string}}) => {
    const [
        courseResponse,
        teamResponse,
        assignmentResponse,
        assignmentsProgressResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeam(params.teamId),
        getTeacherAssignments(params.courseId),
        getTeamAssignmentsProgress(params.courseId, params.teamId)
    ])

    return {
        course: courseResponse.data,
        team: teamResponse.data,
        assignments: assignmentResponse.data,
        assignmentsProgress: assignmentsProgressResponse.data
    }
};