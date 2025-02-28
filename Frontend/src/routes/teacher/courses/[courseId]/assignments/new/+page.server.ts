import { getCourse } from "src/api";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const courseResponse = await getCourse(params.courseId);
    return { 
        course: courseResponse.data,
    }
};