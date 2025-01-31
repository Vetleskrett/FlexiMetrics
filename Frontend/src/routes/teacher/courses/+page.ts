import { getCoursesByTeacher } from "src/api";
import { teacherId } from "src/store";

export const load = async () => {
    const coursesResponse = await getCoursesByTeacher(teacherId);
    return { 
        courses: coursesResponse.data
    }
};