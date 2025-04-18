import { getCourse, getTeacherAssignments, getTeachers, getStudents, getTeams } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse,
        teachersResponse,
        studentsResponse,
        teamsResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeacherAssignments(params.courseId),
        getTeachers(params.courseId),
        getStudents(params.courseId),
        getTeams(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignments: assignmentsResponse.data,
        teachers: teachersResponse.data,
        students: studentsResponse.data,
        teams: teamsResponse.data
    }
};