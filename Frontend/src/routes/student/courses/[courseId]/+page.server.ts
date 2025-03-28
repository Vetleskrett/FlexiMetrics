import { getCourse, getStudentAssignments, getStudentAssignmentsProgress, getStudentTeam, getTeachers, getTeams } from "src/api.server";
import { studentId } from "src/store";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse,
        assignmentsProgressResponse,
        teachersResponse,
        teamResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudentAssignments(params.courseId),
        getStudentAssignmentsProgress(params.courseId, studentId),
        getTeachers(params.courseId),
        getStudentTeam(params.courseId, studentId)
    ])

    let teamsResponse = null;
    if (teamResponse.status == 204) {
        teamsResponse = await getTeams(params.courseId)
    }

    return { 
        course: courseResponse.data,
        assignments: assignmentsResponse.data,
        assignmentsProgress: assignmentsProgressResponse.data,
        team: teamResponse?.data,
        teams: teamsResponse?.data,
        teachers: teachersResponse.data,
    }
};