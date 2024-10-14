export type Assignment = {
    id: string;
    name: string;
    due: string;
    published: boolean;
}

export type Student = {
    id: string;
    email: string;
}

export type Teacher = {
    id: string;
    email: string;
    name: string;
}

export type Team = {
    id: string;
    students: Student[]
}