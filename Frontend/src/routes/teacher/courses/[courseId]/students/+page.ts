import { getCourse, getStudents } from "src/api";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        studentsResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudents(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        students: studentsResponse.data
    }
};