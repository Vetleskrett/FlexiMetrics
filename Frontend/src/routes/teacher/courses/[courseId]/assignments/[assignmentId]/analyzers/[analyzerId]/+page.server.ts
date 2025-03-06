import { getCourse, getAssignment, getStudents, getTeams, getAnalyzer, getAnalyzerAnalyses } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; analyzerId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        analyzerResponse,
        analysesResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAnalyzer(params.analyzerId),
        getAnalyzerAnalyses(params.analyzerId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        analyzer: analyzerResponse.data,
        analyses: analysesResponse.data
    }
};