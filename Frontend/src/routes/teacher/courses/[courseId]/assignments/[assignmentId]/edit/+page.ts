import { getAssignment, getAssignmentFields, getCourse } from "src/api";

export const load = async ({ params }: {params: { courseId: string, assignmentId: string }}) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data
    }
};