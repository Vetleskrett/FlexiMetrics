import { getCourse, getTeachers } from "src/api.server";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        teacherResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeachers(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        teachers: teacherResponse.data
    }
};