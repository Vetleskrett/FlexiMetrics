export type Assignment = {
    id: string;
    name: string;
    due: string;
    individual: boolean;
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
    students: Student[];
}

export type DeliveryField = {
    id: string;
    name: string;
    type: 'String' | 'Integer' | 'Boolean' | 'File';
}

export type DeliveryFieldValue = {
    fieldId: string;
    value: any;
}

export type Delivery = {
    teamId: string;
    values: DeliveryFieldValue[];
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