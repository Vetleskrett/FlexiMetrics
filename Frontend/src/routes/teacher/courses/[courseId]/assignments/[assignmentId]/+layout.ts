import { getAnalyzers } from "src/api";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; } }) => {
    const analyzersResponse = await getAnalyzers(params.assignmentId);
    return { 
        analyzers: analyzersResponse.data
    }
};