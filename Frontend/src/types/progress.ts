export type Progress = {
    id: string;
    assignmentsProgress: AssignmentProgress[];
}

export type AssignmentProgress = {
    id: string;
    isDelivered: boolean;
}