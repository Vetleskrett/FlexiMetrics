import { getAnalyzers } from "src/api.server";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; } }) => {
    const analyzersResponse = await getAnalyzers(params.assignmentId);
    return { 
        analyzers: analyzersResponse.data
    }
};