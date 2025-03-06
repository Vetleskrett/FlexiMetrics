import axios, { type AxiosResponse } from 'axios';
import type {
  Course,
  Assignment,
  StudentAssignment,
  AssignmentField,
  Team,
  Student,
  Delivery,
  Feedback,
  Teacher,
  Analyzer,
  AnalyzerAnalyses,
} from "./types/";

export const api = axios.create({
  baseURL: 'http://localhost:5041'
})

export async function setAuthToken(token: string){
  api.defaults.headers.common["Authorization"] = `Bearer ${token}`
}

export async function hasToken(){
  return api.defaults.headers.common["Authorization"]
}

export async function getCourses() : Promise<AxiosResponse<Course[]>> {
  return api.get(`courses`)
}

export async function getCoursesByTeacher(teacherId: string) : Promise<AxiosResponse<Course[]>> {
  return api.get(`teachers/${teacherId}/courses`)
}

export async function getCoursesByStudent(studentId: string) : Promise<AxiosResponse<Course[]>> {
  return api.get(`students/${studentId}/courses`)
}

export async function getCourse(courseId: string) : Promise<AxiosResponse<Course>> {
  return api.get(`courses/${courseId}`)
}

export async function getAssignments(courseId: string) : Promise<AxiosResponse<Assignment[]>> {
  return api.get(`course/${courseId}/assignments`)
}

export async function getStudentAssignments(studentId: string, courseId: string) : Promise<AxiosResponse<StudentAssignment[]>> {
  return api.get(`students/${studentId}/course/${courseId}/assignments`)
}

export async function getTeamAssignments(courseId: string, teamId: string) : Promise<AxiosResponse<StudentAssignment[]>> {
  return api.get(`courses/${courseId}/teams/${teamId}/assignments`)
}

export async function getAssignment(assignmentId: string): Promise<AxiosResponse<Assignment>> {
  return api.get(`assignments/${assignmentId}`)
}

export async function getAssignmentFields(assignmentId: string): Promise<AxiosResponse<AssignmentField[]>> {
  return api.get(`assignments/${assignmentId}/fields`)
}

export async function getStudentDelivery(studentId: string, assignmentId: string): Promise<AxiosResponse<Delivery>> {
  return api.get(`students/${studentId}/assignments/${assignmentId}/deliveries`)
}

export async function getTeamDelivery(teamId: string, assignmentId: string): Promise<AxiosResponse<Delivery>> {
  return api.get(`teams/${teamId}/assignments/${assignmentId}/deliveries`)
}

export async function getDeliveries(assignmentId: string) : Promise<AxiosResponse<Delivery[]>> {
  return api.get(`assignments/${assignmentId}/deliveries`)
}

export async function getDeliveryFieldFile(deliveryFieldId: string): Promise<AxiosResponse> {
  return api.get(`delivery-fields/${deliveryFieldId}`, { responseType: 'stream' });
}

export async function getFeedbacks(assignmentId: string) : Promise<AxiosResponse<Feedback[]>> {
  return api.get(`assignments/${assignmentId}/feedbacks`)
}

export async function getStudentFeedback(studentId: string, assignmentId: string): Promise<AxiosResponse<Feedback>> {
  return api.get(`students/${studentId}/assignments/${assignmentId}/feedbacks`)
}

export async function getTeamFeedback(teamId: string, assignmentId: string): Promise<AxiosResponse<Feedback>> {
  return api.get(`teams/${teamId}/assignments/${assignmentId}/feedbacks`)
}

export async function getTeam(teamId: string): Promise<AxiosResponse<Team>> {
  return api.get(`teams/${teamId}`)
}

export async function getTeams(courseId: string): Promise<AxiosResponse<Team[]>> {
  return api.get(`courses/${courseId}/teams`)
}

export async function getStudent(studentId: string): Promise<AxiosResponse<Student>> {
  return api.get(`students/${studentId}`)
}

export async function getStudents(courseId: string): Promise<AxiosResponse<Student[]>> {
  return api.get(`courses/${courseId}/students`)
}

export async function getStudentTeam(courseId: string, studentId: string) : Promise<AxiosResponse<Team>>{
  return api.get(`students/${studentId}/courses/${courseId}/teams`)
}

export async function getTeachers(courseId: string): Promise<AxiosResponse<Teacher[]>> {
  return api.get(`courses/${courseId}/teachers`)
}

export async function getAnalyzer(analyzerId: string) : Promise<AxiosResponse<Analyzer>>{
  return api.get(`analyzers/${analyzerId}`)
}

export async function getAnalyzers(assignmentId: string) : Promise<AxiosResponse<Analyzer[]>>{
  return api.get(`assignments/${assignmentId}/analyzers`)
}

export async function getAnalyzerScript(analyzerId: string, stream: boolean = true) : Promise<AxiosResponse>{
  return api.get(`analyzers/${analyzerId}/script`, { responseType: stream ? 'stream' : undefined })
}

export async function getAnalyzerAnalyses(analyzerId: string) : Promise<AxiosResponse<AnalyzerAnalyses>>{
  return api.get(`analyzers/${analyzerId}/analyses`)
}