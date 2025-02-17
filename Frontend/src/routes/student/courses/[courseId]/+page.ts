import { getCourse, getStudentAssignments, getStudentTeam, getTeachers, getTeams } from "src/api";
import { studentId } from "src/store";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse,
        teachersResponse,
        teamResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudentAssignments(studentId, params.courseId),
        getTeachers(params.courseId),
        getStudentTeam(params.courseId, studentId).catch(e => null)
    ])

    let teamsResponse = null;
    if (teamResponse == null) {
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