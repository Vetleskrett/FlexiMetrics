import { browser } from '$app/environment'; 
import axios, { type AxiosResponse } from 'axios';
import type {
  Course,
  Assignment,
  StudentAssignment,
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
  Teacher,
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
} from "./types";

const instance = axios.create({
  baseURL: browser ? 'https://localhost:7255' : 'http://localhost:5041'
})

export function getCourses() : Promise<AxiosResponse<Course[]>> {
  return instance.get("/courses")
}

export function getCoursesByTeacher(teacherId: string) : Promise<AxiosResponse<Course[]>> {
  return instance.get(`/teachers/${teacherId}/courses`)
}

export function getCoursesByStudent(studentId: string) : Promise<AxiosResponse<Course[]>> {
  return instance.get(`/students/${studentId}/courses`)
}

export function getCourse(courseId: string) : Promise<AxiosResponse<Course>> {
  return instance.get(`/courses/${courseId}`)
}

export function getAssignments(courseId: string) : Promise<AxiosResponse<Assignment[]>> {
  return instance.get(`/course/${courseId}/assignments`)
}

export function getStudentAssignments(studentId: string, courseId: string) : Promise<AxiosResponse<StudentAssignment[]>> {
  return instance.get(`/students/${studentId}/course/${courseId}/assignments`)
}

export function getTeamAssignments(courseId: string, teamId: string) : Promise<AxiosResponse<StudentAssignment[]>> {
  return instance.get(`/courses/${courseId}/teams/${teamId}/assignments`)
}

export function getAssignment(assignmentId: string): Promise<AxiosResponse<Assignment>> {
  return instance.get(`/assignments/${assignmentId}`)
}

export function getAssignmentFields(assignmentId: string): Promise<AxiosResponse<AssignmentField[]>> {
  return instance.get(`/assignments/${assignmentId}/fields`)
}

export function getStudentDelivery(studentId: string, assignmentId: string): Promise<AxiosResponse<Delivery>> {
  return instance.get(`/students/${studentId}/assignments/${assignmentId}/deliveries`)
}

export function getTeamDelivery(teamId: string, assignmentId: string): Promise<AxiosResponse<Delivery>> {
  return instance.get(`/teams/${teamId}/assignments/${assignmentId}/deliveries`)
}

export function getDeliveries(assignmentId: string) : Promise<AxiosResponse<Delivery[]>> {
  return instance.get(`/assignments/${assignmentId}/deliveries`)
}

export function getDeliveryFieldFile(deliveryFieldId: string): string {
  return instance.defaults.baseURL + `/delivery-fields/${deliveryFieldId}`;
}

export function getFeedbacks(assignmentId: string) : Promise<AxiosResponse<Feedback[]>> {
  return instance.get(`/assignments/${assignmentId}/feedbacks`)
}

export function postFeedback(feedback: CreateFeedback) : Promise<AxiosResponse<Feedback>> {
  return instance.post(`/feedbacks`, feedback)
}

export function putFeedback(feedbackId: string, feedback: EditFeedback) : Promise<AxiosResponse<Feedback>> {
  return instance.put(`/feedbacks/${feedbackId}`, feedback)
}

export function postDelivery(delivery: CreateDelivery) : Promise<AxiosResponse<Delivery>> {
  return instance.post(`/deliveries`, delivery)
}

export function putDelivery(deliveryId:string, delivery: UpdateDelivery) : Promise<AxiosResponse<Delivery>> {
  return instance.put(`/deliveries/${deliveryId}`, delivery)
}

export function postDeliveryFieldFile(deliveryFieldId: string, file: File) : Promise<AxiosResponse> {
  var formData = new FormData();
  formData.append("file", file);
  return instance.postForm(`/delivery-fields/${deliveryFieldId}`, formData)
}

export function getStudentFeedback(studentId: string, assignmentId: string): Promise<AxiosResponse<Feedback>> {
  return instance.get(`/students/${studentId}/assignments/${assignmentId}/feedbacks`)
}

export function getTeamFeedback(teamId: string, assignmentId: string): Promise<AxiosResponse<Feedback>> {
  return instance.get(`/teams/${teamId}/assignments/${assignmentId}/feedbacks`)
}

export function getTeam(teamId: string): Promise<AxiosResponse<Team>> {
  return instance.get(`/teams/${teamId}`)
}

export function getTeams(courseId: string): Promise<AxiosResponse<Team[]>> {
  return instance.get(`/courses/${courseId}/teams`)
}

export function getStudentTeam(courseId: string, studentId: string) : Promise<AxiosResponse<Team>>{
  return instance.get(`students/${studentId}/courses/${courseId}/teams`)
}

export function postTeams(payload: CreateTeams) : Promise<AxiosResponse> {
  return instance.post("/teams", payload)
}

export function getStudent(studentId: string): Promise<AxiosResponse<Student>> {
  return instance.get(`students/${studentId}`)
}

export function getStudents(courseId: string): Promise<AxiosResponse<Student[]>> {
  return instance.get(`/courses/${courseId}/students`)
}

export function postStudentsCourse(courseId: string, emails: AddStudentsToCourse) : Promise<AxiosResponse<Student[]>> {
  return instance.post(`/courses/${courseId}/students`, emails)
}

export function deleteStudentCourse(courseId: string, studentId: string) : Promise<AxiosResponse> {
  return instance.delete(`/courses/${courseId}/students/${studentId}`)
}

export function postStudentsTeam(teams: AddStudentsToTeams) : Promise<AxiosResponse> {
  return instance.post(`/teams/bulk`, teams)
}

export function postStudentTeam(teamId: string, studentId: string) : Promise<AxiosResponse<Team>>{
  return instance.post(`/teams/${teamId}/students/${studentId}`)
}

export function postStudentEmailTeam(teamId: string, email: EmailAdd) : Promise<AxiosResponse<Team>>{
  return instance.post(`/teams/${teamId}/students/`, email)
}

export function deleteStudentTeam(teamId: string, studentId: string) : Promise<AxiosResponse> {
  return instance.delete(`/teams/${teamId}/students/${studentId}`)
}

export function postCourse(course: CreateCourse) : Promise<AxiosResponse<Course>> {
  return instance.post(`/courses`, course)
}

export function getTeachers(courseId: string): Promise<AxiosResponse<Teacher[]>> {
  return instance.get(`/courses/${courseId}/teachers`)
}

export function addTeacherToCourse(courseId: string, teacher: EmailAdd) : Promise<AxiosResponse> {
  return instance.post(`/courses/${courseId}/teachers`, teacher)
}

export function deleteCourse(courseId: string) : Promise<AxiosResponse> {
  return instance.delete(`/courses/${courseId}`)
}

export function editCourse(courseId: string, course: EditCourse) : Promise<AxiosResponse<Course>> {
  return instance.put(`/courses/${courseId}`, course)
}

export async function postAssignment(assingment: CreateAssignment) : Promise<AxiosResponse<Assignment>>{
  return instance.post(`/assignments`, assingment)
}

export async function editAssignment(assignmentId: string, assignment: EditAssignment) : Promise<AxiosResponse<Assignment>>{
  return instance.put(`/assignments/${assignmentId}`, assignment)
}

export async function publishAssignment(assignmentId: string) : Promise<AxiosResponse<Assignment>>{
  return instance.put(`/assignments/${assignmentId}/publish`,)
}

export async function deleteAssigment(assignmentId: string) : Promise<AxiosResponse>{
  return instance.delete(`/assignments/${assignmentId}`)
}

export async function editAssignmentFields(assignmentId: string, request: UpdateAssignmentFields):Promise<AxiosResponse<AssignmentField[]>> {
  return instance.put(`/assignments/${assignmentId}/fields`, request)
}

export async function deleteAssigmentField(assignmentFieldId: string) : Promise<AxiosResponse>{
  return instance.delete(`/assignment-fields/${assignmentFieldId}`)
}

export async function getAnalyzer(analyzerId: string) : Promise<AxiosResponse<Analyzer>>{
  return instance.get(`/analyzers/${analyzerId}`)
}

export async function getAnalyzers(assignmentId: string) : Promise<AxiosResponse<Analyzer[]>>{
  return instance.get(`/assignments/${assignmentId}/analyzers`)
}

export async function postAnalyzer(request: CreateAnalyzer) : Promise<AxiosResponse<Analyzer>>{
  return instance.post(`/analyzers`,request)
}

export async function editAnalyzer(analyzerId: string, request: EditAnalyzer) : Promise<AxiosResponse<Analyzer>>{
  return instance.put(`/analyzers/${analyzerId}`, request)
}

export async function getAnalyzerScript(analyzerId: string) : Promise<AxiosResponse<File>>{
  return instance.get(`/analyzers/${analyzerId}/script`)
}

export function getAnalyzerScriptUrl(analyzerId: string) : string {
  return instance.defaults.baseURL + `/analyzers/${analyzerId}/script`;
}

export function postAnalyzerScript(analyzerId: string, script: File) : Promise<AxiosResponse> {
  var formData = new FormData();
  formData.append("script", script);
  return instance.postForm(`/analyzers/${analyzerId}/script`, formData)
}