import { getCourse, getAssignment, getStudents, getTeams, getAnalyzer } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; analyzerId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        analyzerResponse,
        studentsResponse,
        teamsResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAnalyzer(params.analyzerId),
        getStudents(params.courseId),
        getTeams(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        analyzer: analyzerResponse.data,
        students: studentsResponse.data,
        teams: teamsResponse.data
    }
};