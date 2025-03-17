import axios, { type AxiosResponse } from 'axios';
import type {
  Course,
  Assignment,
  AssignmentField,
  Team,
  CreateTeams,
  Student,
  AddStudentsToCourse,
  AddStudentsToTeams,
  Delivery,
  Feedback,
  CreateCourse,
  EditCourse,
  CreateDelivery,
  CreateAssignment,
  EditAssignment,
  UpdateAssignmentFields,
  CreateFeedback,
  EditFeedback,
  EmailAdd,
  UpdateDelivery,
  Analyzer,
  CreateAnalyzer,
  EditAnalyzer,
  Analysis,
  AnalyzerAnalyses,
} from "./types/";

export async function postFeedback(feedback: CreateFeedback) : Promise<AxiosResponse<Feedback>> {
  return axios.post(`/api/feedbacks`, feedback)
}

export async function putFeedback(feedbackId: string, feedback: EditFeedback) : Promise<AxiosResponse<Feedback>> {
  return axios.put(`/api/feedbacks/${feedbackId}`, feedback)
}

export async function postDelivery(delivery: CreateDelivery) : Promise<AxiosResponse<Delivery>> {
  return axios.post(`/api/deliveries`, delivery)
}

export async function putDelivery(deliveryId:string, delivery: UpdateDelivery) : Promise<AxiosResponse<Delivery>> {
  return axios.put(`/api/deliveries/${deliveryId}`, delivery)
}

export async function postDeliveryFieldFile(deliveryFieldId: string, file: FormData) : Promise<AxiosResponse> {
  return axios.postForm(`/api/delivery-fields/${deliveryFieldId}`, file)
}

export async function postTeams(payload: CreateTeams) : Promise<AxiosResponse> {
  return axios.post(`/api/teams`, payload)
}

export async function postStudentsCourse(courseId: string, emails: AddStudentsToCourse) : Promise<AxiosResponse<Student[]>> {
  return axios.post(`/api/courses/${courseId}/students`, emails)
}

export async function deleteStudentCourse(courseId: string, studentId: string) : Promise<AxiosResponse> {
  return axios.delete(`/api/courses/${courseId}/students/${studentId}`)
}

export async function postStudentsTeam(teams: AddStudentsToTeams) : Promise<AxiosResponse> {
  return axios.post(`/api/teams/bulk`, teams)
}

export async function postStudentTeam(teamId: string, studentId: string) : Promise<AxiosResponse<Team>>{
  return axios.post(`/api/teams/${teamId}/students/${studentId}`)
}

export async function postStudentEmailTeam(teamId: string, email: EmailAdd) : Promise<AxiosResponse<Team>>{
  return axios.post(`/api/teams/${teamId}/students`, email)
}

export async function deleteStudentTeam(teamId: string, studentId: string) : Promise<AxiosResponse> {
  return axios.delete(`/api/teams/${teamId}/students/${studentId}`)
}

export async function postCourse(course: CreateCourse) : Promise<AxiosResponse<Course>> {
  return axios.post(`/api/courses`, course)
}

export async function addTeacherToCourse(courseId: string, teacher: EmailAdd) : Promise<AxiosResponse> {
  return axios.post(`/api/courses/${courseId}/teachers`, teacher)
}

export async function removeTeacherFromCourse(courseId: string, teacherId: string) : Promise<AxiosResponse> {
  return axios.delete(`/api/courses/${courseId}/teachers/${teacherId}`);
}

export async function deleteCourse(courseId: string) : Promise<AxiosResponse> {
  return axios.delete(`/api/courses/${courseId}`)
}

export async function putCourse(courseId: string, course: EditCourse) : Promise<AxiosResponse<Course>> {
  return axios.put(`/api/courses/${courseId}`, course)
}

export async function postAssignment(assingment: CreateAssignment) : Promise<AxiosResponse<Assignment>>{
  return axios.post(`/api/assignments`, assingment)
}

export async function putAssignment(assignmentId: string, assignment: EditAssignment) : Promise<AxiosResponse<Assignment>>{
  return axios.put(`/api/assignments/${assignmentId}`, assignment)
}

export async function publishAssignment(assignmentId: string) : Promise<AxiosResponse<Assignment>>{
  return axios.put(`/api/assignments/${assignmentId}/publish`,)
}

export async function deleteAssigment(assignmentId: string) : Promise<AxiosResponse>{
  return axios.delete(`/api/assignments/${assignmentId}`)
}

export async function putAssignmentFields(assignmentId: string, request: UpdateAssignmentFields) : Promise<AxiosResponse<AssignmentField[]>> {
  return axios.put(`/api/assignments/${assignmentId}/fields`, request)
}

export async function deleteAssigmentField(assignmentFieldId: string) : Promise<AxiosResponse>{
  return axios.delete(`/api/assignment-fields/${assignmentFieldId}`)
}

export async function postAnalyzer(request: CreateAnalyzer) : Promise<AxiosResponse<Analyzer>>{
  return axios.post(`/api/analyzers`,request)
}

export async function putAnalyzer(analyzerId: string, request: EditAnalyzer) : Promise<AxiosResponse<Analyzer>>{
  return axios.put(`/api/analyzers/${analyzerId}`, request)
}

export async function deleteAnalyzer(analyzerId: string) : Promise<AxiosResponse>{
  return axios.delete(`/api/analyzers/${analyzerId}`)
}

export async function postAnalyzerScript(analyzerId: string, script: FormData) : Promise<AxiosResponse> {
  return axios.postForm(`/api/analyzers/${analyzerId}/script`, script)
}

export async function runAnalyzer(analyzerId: string) : Promise<AxiosResponse> {
  return axios.post(`/api/analyzers/${analyzerId}/action`, {
    action: 'Run'
  });
}

export async function cancelAnalyzer(analyzerId: string) : Promise<AxiosResponse> {
  return axios.post(`/api/analyzers/${analyzerId}/action`, {
    action: 'Cancel'
  });
}

export async function getAnalyzerAnalyses(analyzerId: string) : Promise<AxiosResponse<AnalyzerAnalyses>> {
  return axios.get(`/api/analyzers/${analyzerId}/analyses`)
}

export async function getAnalysis(analysisId: string) : Promise<AxiosResponse<Analysis>> {
  return axios.get(`/api/analyses/${analysisId}`)
}

export async function deleteAnalysis(analysisId: string) : Promise<AxiosResponse> {
  return axios.delete(`/api/analyses/${analysisId}`)
}

export function getAnalyzerStatusEventSource(analyzerId: string) : EventSource {
  return new EventSource(`/api/analyzers/${analyzerId}/status`);
}