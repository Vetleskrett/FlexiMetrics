export enum CollaborationTypeEnum {
    Individual = 0,
    Teams = 1,
}

export enum GradingTypeEnum {
    NoGrading = 0,
    ApprovalGrading = 1,
    LetterGrading = 2,
    PointsGrading = 3,
}

export type CollaborationType = 'Individual' | 'Teams';

export type Assignment = {
    id: string;
    name: string;
    dueDate: string;
    published: boolean;
    collaborationType: CollaborationType;
    courseId : string;
    mandatory: boolean;
    gradingType: GradingType;
    maxPoints?: number;
    description: string;
}

export type GradingType = 'NoGrading' | 'ApprovalGrading' | 'LetterGrading' | 'PointsGrading';

export type AssignmentTeam = {
    id: string;
    name: string;
    due: string;
    completed: boolean;
}

export type AssignmentFieldType = 'ShortText' | 'LongText' | 'Integer' | 'Float' | 'Boolean' | 'URL' | 'JSON' | 'File' | 'List';

export type AssignmentField = {
    id: string;
    name: string;
    type: AssignmentFieldType;
    min?: number;
    max?: number;
    regex?: string;
    subType?: AssignmentFieldType;
}

export type AssignmentFieldFormData = {
    id?: string;
    name: string;
    type: {
        label: string;
        value: AssignmentFieldType;
    };

    min?: number;
    max?: number;
    regex?: string;
    subType?: {
        label: string;
        value: AssignmentFieldType;
    };
};

export type UpdateAssignmentField = {
    id?: string;
    name: string;
    type: AssignmentFieldType;
    min?: number;
    max?: number;
    regex?: string;
    subType?: AssignmentFieldType;
}

export type UpdateAssignmentFields = {
    fields: UpdateAssignmentField[]
}

export type CreateAssignment = {
    name: string,
    description: string,
    courseId: string,
    dueDate: string,
    gradingType: GradingType,
    maxPoints: number | undefined,
    mandatory: boolean,
    published: boolean,
    collaborationType: CollaborationType,
    fields: UpdateAssignmentField[]
}

export type EditAssignment = {
    name: string,
    description: string,
    dueDate: string,
    gradingType: GradingType,
    maxPoints?: number | undefined,
    mandatory: boolean,
    published: boolean,
    collaborationType: CollaborationType,
}