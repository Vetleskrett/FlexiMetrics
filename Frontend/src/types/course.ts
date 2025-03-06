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

export type AddStudentsToCourse = {
    emails: string[],
}