import { getCourse, getStudentAssignments, getTeachers } from "src/api";
import { studentId } from "src/store";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        assignmentsResponse,
        teachersResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudentAssignments(studentId, params.courseId),
        getTeachers(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignments: assignmentsResponse.data,
        team: null,
        teachers: teachersResponse.data,
    }
};