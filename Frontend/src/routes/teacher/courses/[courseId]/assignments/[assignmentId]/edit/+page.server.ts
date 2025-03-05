import { getAssignment, getCourse } from "src/api.server";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ params }: {params: { courseId: string, assignmentId: string }}) => {
    const [
        courseResponse,
        assignmentResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
    }
};