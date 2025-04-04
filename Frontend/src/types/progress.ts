import type { Assignment } from "./assignment";
import type { Feedback } from "./feedback";

export type SlimProgress = {
    id: string;
    assignmentsProgress: SlimAssignmentProgress[];
}

export type SlimAssignmentProgress = {
    id: string;
    isDelivered: boolean;
}

export type AssignmentProgress = {
    assignment: Assignment;
    feedback?: Feedback;
    studentId?: string;
    teamId?: string;
    isDelivered: boolean;
}