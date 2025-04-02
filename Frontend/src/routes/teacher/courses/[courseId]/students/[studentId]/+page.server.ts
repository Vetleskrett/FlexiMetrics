import { getCourse, getStudent, getTeacherAssignments, getStudentAssignmentsProgress, getStudentTeam } from "src/api.server";

export const load = async ({ params }: {params: {courseId: string; studentId: string}}) => {
    const [
        courseResponse,
        studentResponse,
        assignmentsProgressResponse,
        teamResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getStudent(params.studentId),
        getStudentAssignmentsProgress(params.courseId, params.studentId),
        getStudentTeam(params.courseId, params.studentId)
    ])

    return {
        course: courseResponse.data,
        student: studentResponse.data,
        assignmentsProgress: assignmentsProgressResponse.data,
        team: teamResponse?.data
    }
};