import { getCourse, getStudents, getTeam, getTeamAssignments } from "src/api.server";

export const load = async ({ params }: {params: {courseId: string; teamId: string}}) => {
    const [
        courseResponse,
        teamResponse,
        assignmentResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeam(params.teamId),
        getTeamAssignments(params.courseId, params.teamId)
    ])

    return {
        course: courseResponse.data,
        team: teamResponse.data,
        assignments: assignmentResponse.data
    }
};