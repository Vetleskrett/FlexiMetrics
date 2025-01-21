
export type Course = {
    id: string;
    name: string;
    code: string;
    year: number;
    semester: Semester;
    numStudents: number | null | undefined
    numTeams: number | null | undefined
    teachers: Teacher[] | null | undefined
}

enum Semester{
    Spring = "Spring",
    Fall = "Fall",
}

export enum Role {
    Teacher = 0,
    Student = 1,
}

export type Assignment = {
    id: string;
    name: string;
    dueDate: string;
    published: boolean;
    collaborationType : string;
    courseId : string;
}

export type AssignmentTeam = {
    id: string;
    name: string;
    due: string;
    completed: boolean;
}

export type Student = {
    id: string;
    name: string;
    email: string;
}

export type Teacher = {
    id: string;
    email: string;
    name: string;
}

export type Team = {
    id: string;
    students: Student[];
    teamNr: number;
    complete: number | null | undefined;
}

export type AssignmentField = {
    id: string;
    name: string;
    type: 'String' | 'Integer' | 'Boolean' | 'File';
}

export type DeliveryFieldValue = {
    fieldId: string;
    value: any;
}

export type Delivery = {
    teamId: string;
    values: DeliveryFieldValue[];
}

export type Analyzer = {
    id: string;
    name: string;
}

export type AnalyzerFieldType = 'String' | 'Integer' | 'Boolean' | 'File' | 'List' | 'Json' | 'Range';

export type AnalyzerField = {
    id: string;
    name: string;
} &
(
    {
        type: 'String' | 'Integer' | 'Boolean' | 'File' | 'List' | 'Json';
    } |
    {
        type: 'Range';
        max: number;
    }
)

export type AnalyzerTeamOutput = {
    teamId: string;
    values: Map<string, any>;
}

export type AnalyzerOutputVersion = {
    id: string;
    datetime: Date;
}

export type AnalyzerOutput = {
    versions: AnalyzerOutputVersion[];
    currentVersion: AnalyzerOutputVersion;
    fields: AnalyzerField[];
    teamOutputs: AnalyzerTeamOutput[];
}

export type CreateTeams = {
    courseId: string;
    numTeams: number;
}