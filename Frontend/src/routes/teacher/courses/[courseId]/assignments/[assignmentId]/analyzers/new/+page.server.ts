import { getCourse, getAssignment } from "src/api.server";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data
    }
};