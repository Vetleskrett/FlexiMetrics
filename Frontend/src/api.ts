import { browser } from '$app/environment'; 
import axios, { type AxiosResponse } from 'axios';
import type {
  Course,
  TeacherCourse,
  StudentCourse,
  Assignment,
  StudentAssignment,
  AssignmentField,
  Team,
  CreateTeams,
  Student,
  AddStudentsToCourse,
  AddStudentsToTeams,
  Delivery, Feedback,
  CreateCourse,
  AddTeacherToCourse,
  CreateDelivery
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

export function getTeacherCourse(teacherId: string, courseId: string) : Promise<AxiosResponse<TeacherCourse>> {
  return instance.get(`/teachers/${teacherId}/courses/${courseId}`)
}

export function getStudentCourse(studentId: string, courseId: string) : Promise<AxiosResponse<StudentCourse>> {
  return instance.get(`/students/${studentId}/courses/${courseId}`)
}

export function getAssignments(courseId: string) : Promise<AxiosResponse<Assignment[]>> {
  return instance.get(`/course/${courseId}/assignments`)
}

export function getStudentAssignments(studentId: string, courseId: string) : Promise<AxiosResponse<StudentAssignment[]>> {
  return instance.get(`/students/${studentId}/course/${courseId}/assignments`)
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

export function postDelivery(delivery: CreateDelivery) : Promise<AxiosResponse<Delivery>> {
  return instance.post(`/deliveries`, delivery)
}

export function getStudentFeedback(studentId: string, assignmentId: string): Promise<AxiosResponse<Feedback>> {
  return instance.get(`/students/${studentId}/assignments/${assignmentId}/feedbacks`)
}

export function getTeams(courseId: string): Promise<AxiosResponse<Team[]>> {
  return instance.get(`/courses/${courseId}/teams`)
}

export function postTeams(payload: CreateTeams) : Promise<AxiosResponse> {
  return instance.post("/teams", payload)
}

export function getStudents(courseId: string): Promise<AxiosResponse<Student[]>> {
  return instance.get(`/courses/${courseId}/students`)
}

export function postStudentsCourse(courseId: string, emails: AddStudentsToCourse) : Promise<AxiosResponse> {
  return instance.post(`/courses/${courseId}/students`, emails)
}

export function postStudentsTeam(teams: AddStudentsToTeams) : Promise<AxiosResponse> {
  return instance.post(`/teams/bulk`, teams)
}

export function postCourse(course: CreateCourse) : Promise<AxiosResponse<Course>> {
  return instance.post(`/courses`, course)
}

export function addTeacherToCourse(courseId: string, teacher: AddTeacherToCourse) : Promise<AxiosResponse> {
  return instance.post(`/courses/${courseId}/teachers`, teacher)
}

export function deleteCourse(courseId: string) : Promise<AxiosResponse> {
  return instance.delete(`/courses/${courseId}`)
}

export function editCourse(courseId: string, course: CreateCourse) : Promise<AxiosResponse> {
  return instance.put(`/courses/${courseId}`, course)
}
