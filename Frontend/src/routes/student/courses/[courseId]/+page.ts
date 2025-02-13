import { getCourse, getStudentAssignments, getStudentTeam, getTeachers, getTeams } from "src/api";
import { studentId } from "src/store";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse,
        teachersResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudentAssignments(studentId, params.courseId),
        getTeachers(params.courseId),
    ])

    let teams = null
    let studentTeam = await getStudentTeam(params.courseId, studentId)
    if (studentTeam.status == 204){
        teams = await getTeams(params.courseId)
    }


    return { 
        course: courseResponse.data,
        assignments: assignmentsResponse.data,
        team: studentTeam.status == 204 ? null : studentTeam.data,
        teams: teams ? teams.data : null,
        teachers: teachersResponse.data,
    }
};