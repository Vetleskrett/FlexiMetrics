import { getAssignment, getAssignmentFields, getCourse } from "src/api";

export const load = async ({ params }: {params: { courseId: string, assignmentId: string }}) => {
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