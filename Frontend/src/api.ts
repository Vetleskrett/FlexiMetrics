import { assignments } from "./mockData";
import type { Course, Assignment, AssignmentField } from "./types";
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