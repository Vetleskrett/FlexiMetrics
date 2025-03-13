import type { Team } from "./team";
import type { Student } from "./user";

export type Analyzer = {
    id: string;
    name: string;
    fileName: string;
    assignmentId: string;
}

export type AnalysisStatus = 'Started' | 'Running' | 'Completed';

export type AnalyzerAnalyses = {
    analyses: SlimAnalysis[];
    latest?: Analysis;
}

export type SlimAnalysis = {
    id: string;
    startedAt: Date;
    completedAt?: Date;
    status: AnalysisStatus;
    analyzerId: string;
}

export type Analysis = {
    id: string;
    startedAt: Date;
    completedAt?: Date;
    status: AnalysisStatus;
    analyzerId: string;
    analysisEntries: AnalysisEntry[];
    totalNumEntries: number;
}

export type AnalysisEntry = {
    id: string;
    analysisId: string;
    team?: Team;
    student?: Student;
    fields: AnalysisField[];
    logInformation: string;
    logError: string;
    completedAt: Date;
}

export type StudentAnalysis = {
    id: string;
    analyzerName: string;
    completedAt: Date;
    fields: AnalysisField[];
}

export type AnalysisFieldType = 'String' | 'Int';

export type AnalysisField = {
    id: string;
    name: string;
    type: AnalysisFieldType;
    value: any;
}

export type CreateAnalyzer = {
    name: string;
    fileName: string;
    assignmentId: string;
}

export type EditAnalyzer = {
    name: string;
    fileName: string;
}