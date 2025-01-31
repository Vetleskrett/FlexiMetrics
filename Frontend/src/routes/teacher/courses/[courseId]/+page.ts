import { getTeacherCourse, getAssignments } from "src/api";
import { teacherId } from "src/store";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse
    ] = await Promise.all([
        getTeacherCourse(teacherId, params.courseId),
        getAssignments(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignments: assignmentsResponse.data
    }
};