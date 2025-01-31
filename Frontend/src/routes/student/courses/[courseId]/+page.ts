import { getStudentCourse, getStudentAssignments } from "src/api";
import { studentId } from "src/store";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse
    ] = await Promise.all([
        getStudentCourse(studentId, params.courseId),
        getStudentAssignments(studentId, params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignments: assignmentsResponse.data
    }
};