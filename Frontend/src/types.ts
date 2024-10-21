export type Assignment = {
    id: string;
    name: string;
    due: string;
    individual: boolean;
    published: boolean;
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
    complete: number | null | undefined;
}

export type DeliveryFieldType = 'String' | 'Integer' | 'Boolean' | 'File';

export type DeliveryField = {
    id: string;
    name: string;
    type: DeliveryFieldType;
}

export type FieldDelivery = {
    fieldId: string;
    value: any;
}

export type Delivery = {
    teamId: string;
    fields: FieldDelivery[];
}

export type Analyzer = {
    id: string;
    name: string;
}