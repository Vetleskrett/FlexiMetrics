import type { AnalyzerOutput } from "./types";

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