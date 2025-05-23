export enum Role {
    Teacher = 0,
    Student = 1,
}

export type Student = {
    id: string;
    name: string;
    email: string;
}

export type CourseStudent = {
    id: string;
    name: string;
    email: string;
    teamNr?: number;
}

export type Teacher = {
    id: string;
    email: string;
    name: string;
}