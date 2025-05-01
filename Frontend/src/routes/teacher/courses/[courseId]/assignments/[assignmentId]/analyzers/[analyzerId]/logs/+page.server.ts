import { getCourse, getAssignment, getAnalyzer, getAnalyzerAnalyses, getAnalyzerLogs } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; analyzerId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        analyzerResponse,
        analyzerLogsResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAnalyzer(params.analyzerId),
        getAnalyzerLogs(params.analyzerId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        analyzer: analyzerResponse.data,
        analyzerLogs: analyzerLogsResponse.data
    }
};