import { assignments, course } from "./mockData";
import type { Course, Assignment, AssignmentField, Team, CreateTeams, Student, AddStudentsToCourse, AddStudentsToTeams } from "./types";
const API_BASE_URL = 'https://localhost:7255';

async function getApiCall(url:string){
  try {
    const response = await fetch(`${API_BASE_URL + url}`);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching data:', error);
    throw error;
  }
}

async function postApiCall(url:string, payload: any){
  try {
    const response = await fetch(`${API_BASE_URL + url}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', },
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    return;
  } catch (error) {
    console.error('Error fetching data:', error);
    throw error;
  }
}

export async function getCourses() : Promise<Course[]> {
  return getApiCall("/courses")
}

export async function getCourse(courseId: string) : Promise<Course> {
  return getApiCall(`/courses/${courseId}`)
}

export async function getAssignments(courseId: string) : Promise<Assignment[]> {
  return getApiCall(`/course/${courseId}/assignments`)
}

export async function getAssignment(assignmentId: string): Promise<Assignment> {
  return getApiCall(`/assignments/${assignmentId}`)
}

export async function getAssignmentFields(assignmentId: string): Promise<AssignmentField[]> {
  return getApiCall(`/assignments/${assignmentId}/fields`)
}

export async function getTeams(courseId: string): Promise<Team[]> {
  return getApiCall(`/courses/${courseId}/teams`)
}

export async function postTeams(payload: CreateTeams) {
  return postApiCall("/teams", payload)
}

export async function getStudents(courseId: string): Promise<Student[]> {
  return getApiCall(`/courses/${courseId}/students`)
}

export async function postStudentsCourse(courseId: string, emails: AddStudentsToCourse) {
  return postApiCall(`/courses/${courseId}/students`, emails)
}
export async function postStudentsTeam(teams: AddStudentsToTeams) {
  return postApiCall(`/teams/bulk`, teams)
}
