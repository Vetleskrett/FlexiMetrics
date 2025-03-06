import type { Student } from "./user";

export type Team = {
    id: string;
    students: Student[];
    teamNr: number;
    complete: number | null | undefined;
}

export type CreateTeams = {
    courseId: string;
    numTeams: number;
}

export type AddStudentsToTeams = {
    courseId: string,
    teams: StudentToTeam[],
}

export type StudentToTeam = {
    teamNr: number,
    emails: string[],
}