export type DeliveryField = {
    id: string;
    assignmentFieldId: string;
    value: any;
}

export type Delivery = {
    id: string;
    assignmentId: string;
    studentId?: string;
    teamId: string;
    fields: DeliveryField[];
}

export type CreateDelivery = {
    assignmentId: string;
    studentId: string;
    fields: CreateDeliveryField[];
}

export type UpdateDelivery = {
    fields: CreateDeliveryField[];
}

export type CreateDeliveryField = {
    assignmentFieldId: string;
    value: any;
}