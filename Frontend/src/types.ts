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

export type FileMetadata = {
    FileName: string;
    ContentType: string;
};

export type Analyzer = {
    id: string;
    name: string;
    fileName: string;
    assignmentId: string;
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

export type EditCourse = {
    code: string,
    name: string,
    year: number
    semester: string,
}

export type EmailAdd = {
    email: string,
}

export type CreateDelivery = {
    assignmentId: string;
    studentId: string;
    fields: CreateDeliveryField[];
}

export type UpdateDelivery = {
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
    maxPoints: number | undefined,
    mandatory: boolean,
    published: boolean,
    collaborationType: CollaborationType,
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

export type CreateAnalyzer = {
    name: string;
    fileName: string;
    assignmentId: string;
}

export type EditAnalyzer = {
    name: string;
    fileName: string;
}