import { getCoursesByStudent } from "src/api.server";
import { studentId } from "src/store";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
    const coursesResponse = await getCoursesByStudent(studentId);
    return { 
        courses: coursesResponse.data
    }
};