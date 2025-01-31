import { getCourse } from "src/api";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const courseResponse = await getCourse(params.courseId);
    return { 
        course: courseResponse.data
    }
};