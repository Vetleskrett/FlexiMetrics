import { getCourse, getStudents, getStudentsProgress } from "src/api.server";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        studentsResponse,
        studentsProgressResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudents(params.courseId),
        getStudentsProgress(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        students: studentsResponse.data,
        studentsProgress: studentsProgressResponse.data
    }
};