import { getCoursesByStudent } from "src/api";
import { studentId } from "src/store";

export const load = async () => {
    const coursesResponse = await getCoursesByStudent(studentId);
    return { 
        courses: coursesResponse.data
    }
};