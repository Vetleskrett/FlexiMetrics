import { getCourse, getAssignment, getFeedbacks, getStudents, getTeams } from "src/api.server";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ params }: { params: { courseId: string; assignmentId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        feedbacksResponse,
        studentsResponse,
        teamsResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getFeedbacks(params.assignmentId),
        getStudents(params.courseId),
        getTeams(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        feedbacks: feedbacksResponse.data,
        students: studentsResponse.data,
        teams: teamsResponse.data
    }
};