import type { Analyzer, Delivery, DeliveryField, Student, Teacher, Team } from "./types";

export const courses = [
    {
        id: '1',
        code: 'TDT101',
        name: 'Programmering',
        year: 2024,
        semester: 'Autumn'
    },
    {
        id: '2',
        code: 'TDT102',
        name: 'Databaser',
        year: 2024,
        semester: 'Autumn'
    },
    {
        id: '3',
        code: 'TDT103',
        name: 'Nettverk',
        year: 2024,
        semester: 'Autumn'
    }
];

export const course = courses[0];

export const assignments = [
    {
        id: '1',
        name: 'Assignment 1',
        due: '18.09.2024',
        individual: false,
        published: true
    },
    {
        id: '2',
        name: 'Assignment 2',
        due: '08.10.2024',
        individual: false,
        published: true
    },
    {
        id: '3',
        name: 'Assignment 3',
        due: '18.10.2024',
        individual: false,
        published: false
    },
    {
        id: '4',
        name: 'Assignment 4',
        due: '08.11.2024',
        individual: false,
        published: false
    }
];

export const assignment = assignments[0];

export const students: Student[] = [];
for (let i = 1; i <= 196; i++) {
    students.push({
        id: i.toString(),
        email: 'ola@ntnu.no'
    });
}

export const teams: Team[] = [];
for (let i = 1; i <= 49; i++) {
    teams.push({
        id: i.toString(),
        students: []
    });
}

export const teachers: Teacher[] = [
    {
        id: '1',
        name: "Ola Nordmann",
        email: "ola@ntnu.no"
    },
    {
        id: '2',
        name: "Kari Nordmann",
        email: "kari@ntnu.no"
    }
];

export const deliveryFields: DeliveryField[] = [
    {
        id: '1',
        name: 'Project Title',
        type: 'String'
    },
    {
        id: '2',
        name: 'Source Code',
        type: 'File'
    },
    {
        id: '3',
        name: 'Url',
        type: 'String'
    }
];

export const deliveries: Delivery[] = [
    {
        teamId: '1',
        fields: new Map([
            ['1', 'Weather app'],
            ['2', 'team1.zip'],
            ['3', 'http://team1.ntnu.no']
        ])
    },
    {
        teamId: '2',
        fields: new Map([
            ['1', 'Snake Game'],
            ['2', 'team2.zip'],
            ['3', 'http://team2.ntnu.no']
        ])
    },
    {
        teamId: '3',
        fields: new Map([
            ['1', 'Tic Tac Toe'],
            ['2', 'team3.zip'],
            ['3', 'http://team3.ntnu.no']
        ])
    },
    {
        teamId: '4',
        fields: new Map([
            ['1', 'Sudoku'],
            ['2', 'team4.zip'],
            ['3', 'http://team4.ntnu.no']
        ])
    },
    {
        teamId: '5',
        fields: new Map([
            ['1', 'Music app'],
            ['2', 'team5.zip'],
            ['3', 'http://team5.ntnu.no']
        ])
    },
    {
        teamId: '6',
        fields: new Map([
            ['1', 'Quiz Game'],
            ['2', 'team6.zip'],
            ['3', 'http://team6.ntnu.no']
        ])
    }
];

export const analyzers: Analyzer[] = [
    {
        id: '1',
        name: 'Git Analyzer'
    },
    {
        id: '2',
        name: 'Lighthouse Analyzer'
    },
    {
        id: '3',
        name: 'Code Analyzer'
    }
];