import { getCourse, getAssignment, getAnalyzer, getAnalyzerScript } from "src/api";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; analyzerId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        analyzerResponse,
        scriptResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAnalyzer(params.analyzerId),
        getAnalyzerScript(params.analyzerId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        analyzer: analyzerResponse.data,
        script: scriptResponse.data
    }
};