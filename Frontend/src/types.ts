
export type Course = {
    id: string;
    name: string;
    code: string;
    year: number;
    semester: Semester;
}

export type TeacherCourse = {
    id: string;
    name: string;
    code: string;
    year: number;
    semester: Semester;
    numStudents: number;
    numTeams: number;
    teachers: Teacher[];
}

export type StudentCourse = {
    id: string;
    name: string;
    code: string;
    year: number;
    semester: Semester;
    team: Team;
    teachers: Teacher[];
}

export enum Semester{
    Spring = "Spring",
    Autumn = "Autumn",
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

export type AssignmentField = {
    id: string;
    name: string;
    type: 'String' | 'Integer' | 'Boolean' | 'File';
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
    semester: number
}

export type AddTeacherToCourse = {
    email: string,
}