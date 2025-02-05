import { getCourse, getAssignment, getAssignmentFields, getDeliveries, getStudents, getTeams } from "src/api";

export const load = async ({ params }: { params: { courseId: string; assignmentId: string; } }) => {
    const [
        courseResponse,
        assignmentResponse,
        assignmentFieldsResponse,
        deliveriesResponse,
        studentsResponse,
        teamsResponse,
    ] = await Promise.all([
        getCourse(params.courseId),
        getAssignment(params.assignmentId),
        getAssignmentFields(params.assignmentId),
        getDeliveries(params.assignmentId),
        getStudents(params.courseId),
        getTeams(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        assignment: assignmentResponse.data,
        assignmentFields: assignmentFieldsResponse.data,
        deliveries: deliveriesResponse.data,
        students: studentsResponse.data,
        teams: teamsResponse.data
    }
};