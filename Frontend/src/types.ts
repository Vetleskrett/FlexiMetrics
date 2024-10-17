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