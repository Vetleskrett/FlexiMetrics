import { getCoursesByTeacher } from "src/api";
import { teacherId } from "src/store";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
    const coursesResponse = await getCoursesByTeacher(teacherId);
    return { 
        courses: coursesResponse.data
    }
};