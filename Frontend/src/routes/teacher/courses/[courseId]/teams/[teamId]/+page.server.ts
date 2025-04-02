import { getCourse, getTeam, getTeacherAssignments, getTeamAssignmentsProgress } from "src/api.server";

export const load = async ({ params }: {params: {courseId: string; teamId: string}}) => {
    const [
        courseResponse,
        teamResponse,
        assignmentsProgressResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeam(params.teamId),
        getTeamAssignmentsProgress(params.courseId, params.teamId)
    ])

    return {
        course: courseResponse.data,
        team: teamResponse.data,
        assignmentsProgress: assignmentsProgressResponse.data
    }
};