import { getCourse, getAssignments, getTeachers, getStudents, getTeams } from "src/api";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ params }) => {
    const [
        courseResponse,
        assignmentsResponse,
        teachersResponse,
        studentsResponse,
        teamsResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignments(params.courseId),
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