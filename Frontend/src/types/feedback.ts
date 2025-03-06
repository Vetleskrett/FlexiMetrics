export type LetterGrade = 'A' | 'B' | 'C' | 'D' | 'E' | ' F';

export type Feedback = {
    id: string;
    comment: string;
    assignmentId: string;
    studentId?: string;
    teamId?: string;
    isApproved?: boolean;
    letterGrade?: LetterGrade;
    points?: number;
}

export type CreateFeedback = {
    comment: string;
    assignmentId: string;
    studentId?: string;
    teamId?: string;
    isApproved?: boolean;
    letterGrade?: LetterGrade;
    points?: number;
}

export type EditFeedback = {
    comment: string;
    isApproved?: boolean;
    letterGrade?: LetterGrade;
    points?: number;
}