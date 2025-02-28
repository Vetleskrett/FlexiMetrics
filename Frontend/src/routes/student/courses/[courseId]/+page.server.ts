import { getCourse, getStudentAssignments, getStudentTeam, getTeachers, getTeams } from "src/api";
import { studentId } from "src/store";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse,
        teachersResponse,
        teamResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudentAssignments(studentId, params.courseId),
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
        team: teamResponse?.data,
        teams: teamsResponse?.data,
        teachers: teachersResponse.data,
    }
};