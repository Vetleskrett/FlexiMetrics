export type Course = {
    id: string;
    name: string;
    code: string;
    year: number;
    semester: Semester;
}

export enum Semester{
    Spring = "Spring",
    Autumn = "Autumn",
}

export enum Role {
    Teacher = 0,
    Student = 1,
}

export enum CollaborationType {
    Individual = 0,
    Teams = 1,
}

export enum GradingTypeEnum {
    NoGrading = 0,
    ApprovalGrading = 1,
    LetterGrading = 2,
    PointsGrading = 3,
}

export type Assignment = {
    id: string;
    name: string;
    dueDate: string;
    published: boolean;
    collaborationType: 'Individual' | 'Teams';
    courseId : string;
    mandatory: boolean;
    gradingType: GradingType;
    maxPoints?: number;
    description: string;
}

export type GradingType = 'NoGrading' | 'ApprovalGrading' | 'LetterGrading' | 'PointsGrading';

export type StudentAssignment = {
    id: string;
    name: string;
    dueDate: string;
    published: boolean;
    collaborationType : string;
    isDelivered: boolean;
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

export type AssignmentFieldType = 'String' | 'Integer' | 'Double' | 'Boolean' | 'File';

export type AssignmentField = {
    id: string;
    name: string;
    type: AssignmentFieldType;
}

export type NewAssignmentField = {
    name: string;
    type: string;
}

export type DeliveryField = {
    id: string;
    assignmentFieldId: string;
    value: any;
}

export type Delivery = {
    id: string;
    assignmentId: string;
    studentId?: string;
    teamId: string;
    fields: DeliveryField[];
}

export type Feedback = {
    id: string;
    comment: string;
    deliveryId: string;
    isApproved?: boolean;
    letterGrade?: 'A' | 'B' | 'C' | 'D' | 'E' | ' F';
    points?: number;
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
export type AddStudentsToCourse = {
    emails: string[],
}

export type AddStudentsToTeams = {
    courseId: string,
    teams: StudentToTeam[],
}
export type StudentToTeam = {
    teamNr: number,
    emails: string[],
}
export type CreateCourse = {
    code: string,
    name: string,
    year: number
    semester: string,
    teacherId: string
}

export type AddTeacherToCourse = {
    email: string,
}

export type CreateDelivery = {
    assignmentId: string;
    studentId: string;
    fields: CreateDeliveryField[];
}

export type CreateDeliveryField = {
    assignmentFieldId: string;
    value: any;
}
export type CreateAssignment = {
    name: string,
    description: string,
    courseId: string,
    dueDate: string,
    gradingType: number,
    maxPoints: number | undefined,
    mandatory: boolean,
    published: boolean,
    collaborationType: number,
    fields: NewAssignmentField[]
}

export type EditAssignment = {
    name: string,
    description: string,
    courseId: string,
    dueDate: string,
    gradingType: number,
    maxPoints: number | undefined,
    mandatory: boolean,
    published: boolean,
    collaborationType: number,
}


