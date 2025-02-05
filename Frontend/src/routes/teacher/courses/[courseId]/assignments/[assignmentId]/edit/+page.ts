import { getAssignment } from "src/api";

export const load = async ({ params }: {params: {assignmentId: string}}) => {
    const assignmentResponse = await getAssignment(params.assignmentId);
    return { 
        assignment: assignmentResponse.data
    }
};