import type { Analyzer, Delivery, AssignmentField, Student, Teacher, Team, AnalyzerOutput } from "./types";

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
        email: 'ola@ntnu.no',
        name: 'ola',
    });
}

export const teams: Team[] = [];
for (let i = 1; i <= 49; i++) {
    teams.push({
        id: i.toString(),
        teamNr: i,
        students: [],
        complete: 2
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

export const assignmentFields: AssignmentField[] = [
    {
        id: '1',
        name: 'Project Title',
        type: 'ShortText'
    },
    {
        id: '2',
        name: 'Source Code',
        type: 'File'
    },
    {
        id: '3',
        name: 'Url',
        type: 'ShortText'
    },
    {
        id: '4',
        name: 'Number',
        type: 'Integer'
    },
    {
        id: '5',
        name: 'True / False',
        type: 'Boolean'
    }
];

export const deliveries: Delivery[] = [
    {
        id: '1',
        assignmentId: '1',
        teamId: '1',
        studentId: undefined,
        fields: [
            {
                id: '1',
                assignmentFieldId: '1',
                value: 'Weather app'
            },
            {
                id: '2',
                assignmentFieldId: '2',
                value: 'team1.zip'
            },
            {
                id: '3',
                assignmentFieldId: '3',
                value: 'http://team1.ntnu.no'
            },
            {
                id: '4',
                assignmentFieldId: '4',
                value: 17
            },
            {
                id: '5',
                assignmentFieldId: '5',
                value: true
            }
        ]
    },
    {
        id: '1',
        assignmentId: '1',
        teamId: '2',
        studentId: undefined,
        fields: [
            {
                id: '6',
                assignmentFieldId: '1',
                value: 'Snake Game'
            },
            {
                id: '7',
                assignmentFieldId: '2',
                value: 'team2.zip'
            },
            {
                id: '8',
                assignmentFieldId: '3',
                value: 'http://team2.ntnu.no'
            },
            {
                id: '9',
                assignmentFieldId: '4',
                value: 65
            },
            {
                id: '10',
                assignmentFieldId: '5',
                value: true
            }
        ]
    },
    {
        id: '1',
        assignmentId: '1',
        teamId: '3',
        studentId: undefined,
        fields: [
            {
                id: '11',
                assignmentFieldId: '1',
                value: 'Tic Tac Toe'
            },
            {
                id: '12',
                assignmentFieldId: '2',
                value: 'team3.zip'
            },
            {
                id: '13',
                assignmentFieldId: '3',
                value: 'http://team3.ntnu.no'
            },
            {
                id: '14',
                assignmentFieldId: '4',
                value: 12
            },
            {
                id: '15',
                assignmentFieldId: '5',
                value: false
            }
        ]
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

export const analyzer: Analyzer = analyzers[0];

export const analyzerOutput: AnalyzerOutput = {
    versions: [
        {
            id: '1',
            datetime: new Date(2024, 3, 21, 17, 26, 12)
        },
        {
            id: '2',
            datetime: new Date(2024, 2, 8, 15, 32, 8)
        },
        {
            id: '3',
            datetime: new Date(2024, 1, 5, 8, 5, 44)
        },
        {
            id: '4',
            datetime: new Date(2024, 0, 1, 12, 43, 34)
        }
    ],
    currentVersion: {
        id: '1',
        datetime: new Date(2024, 3, 21, 17, 26, 12)
    },
    fields: [
        {
            id: '1',
            name: 'Project Title',
            type: 'String'
        },
        {
            id: '2',
            name: 'Report',
            type: 'File'
        },
        {
            id: '3',
            name: 'Members',
            type: 'List'
        },
        {
            id: '4',
            name: 'Json output',
            type: 'Json'
        },
        {
            id: '5',
            name: 'Issues closed',
            type: 'Range',
            max: 100
        }
    ],
    teamOutputs: [
        {
            teamId: '1',
            values: new Map<string, any>([
                ['1', 'Weather app'],
                ['2', 'team1.html'],
                ['3', ['Ola Nordmann', 'Ola Nordmann', 'Ola Nordmann']],
                ['4', 
                    `
                    {
                        "commits": 34,
                        "issues": 11,
                        "contributors": 3,
                        "stars": 1
                    }
                    `],
                ['5', 78],
            ])
        },
        {
            teamId: '2',
            values: new Map<string, any>([
                ['1', 'Snake Game'],
                ['2', 'team2.html'],
                ['3', ['Ola Nordmann', 'Ola Nordmann', 'Ola Nordmann']],
                ['4', 
                    `
                    {
                        "commits": 34,
                        "issues": 11,
                        "contributors": 3,
                        "stars": 1
                    }
                    `],
                ['5', 34],
            ])
        },
        {
            teamId: '3',
            values: new Map<string, any>([
                ['1', 'Tic Tac Toe'],
                ['2', 'team3.html'],
                ['3', ['Ola Nordmann', 'Ola Nordmann', 'Ola Nordmann']],
                ['4', 
                    `
                    {
                        "commits": 34,
                        "issues": 11,
                        "contributors": 3,
                        "stars": 1
                    }
                    `],
                ['5', 100],
            ])
        },
        {
            teamId: '4',
            values: new Map<string, any>([
                ['1', 'Sudoku'],
                ['2', 'team4.html'],
                ['3', ['Ola Nordmann', 'Ola Nordmann', 'Ola Nordmann']],
                ['4', 
                    `
                    {
                        "commits": 34,
                        "issues": 11,
                        "contributors": 3,
                        "stars": 1
                    }
                    `],
                ['5', 12],
            ])
        },
        {
            teamId: '5',
            values: new Map<string, any>([
                ['1', 'Music app'],
                ['2', 'team5.html'],
                ['3', ['Ola Nordmann', 'Ola Nordmann', 'Ola Nordmann']],
                ['4', 
                    `
                    {
                        "commits": 34,
                        "issues": 11,
                        "contributors": 3,
                        "stars": 1
                    }
                    `],
                ['5', 50],
            ])
        },
        {
            teamId: '6',
            values: new Map<string, any>([
                ['1', 'Quiz Game'],
                ['2', 'team6.html'],
                ['3', ['Ola Nordmann', 'Ola Nordmann', 'Ola Nordmann']],
                ['4', 
                    `
                    {
                        "commits": 34,
                        "issues": 11,
                        "contributors": 3,
                        "stars": 1
                    }
                    `],
                ['5', 89],
            ])
        }
    ]
}