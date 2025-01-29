import type { AddTeacherToCourse, CreateCourse, Course, TeacherCourse, StudentCourse, Assignment, AssignmentField, Team, CreateTeams, Student, AddStudentsToCourse, AddStudentsToTeams } from "./types";
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
    return response;
  } catch (error) {
    console.error('Error posting data:', error);
    throw error;
  }
}

async function putApiCall(url:string, payload: any){
  try {
    const response = await fetch(`${API_BASE_URL + url}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', },
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    return response;
  } catch (error) {
    console.error('Error posting data:', error);
    throw error;
  }
}

async function deleteApiCall(url:string){
  try {
    const response = await fetch(`${API_BASE_URL + url}`, { method: 'DELETE' });
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    return;
  } catch (error) {
    console.error('Error deleting data:', error);
    throw error;
  }
}

export async function getCourses() : Promise<Course[]> {
  return getApiCall("/courses")
}

export async function getCoursesByTeacher(teacherId: string) : Promise<Course[]> {
  return getApiCall(`/teachers/${teacherId}/courses`)
}

export async function getCoursesByStudent(studentId: string) : Promise<Course[]> {
  return getApiCall(`/students/${studentId}/courses`)
}

export async function getCourse(courseId: string) : Promise<Course> {
  return getApiCall(`/courses/${courseId}`)
}

export async function getTeacherCourse(teacherId: string, courseId: string) : Promise<TeacherCourse> {
  return getApiCall(`/teachers/${teacherId}/courses/${courseId}`)
}

export async function getStudentCourse(studentId: string, courseId: string) : Promise<StudentCourse> {
  return getApiCall(`/students/${studentId}/courses/${courseId}`)
}

export async function getAssignments(courseId: string) : Promise<Assignment[]> {
  return getApiCall(`/course/${courseId}/assignments`)
}


export async function getStudentAssignments(studentId: string, courseId: string) : Promise<StudentAssignment[]> {
  return getApiCall(`/students/${studentId}/course/${courseId}/assignments`)
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

export async function postCourse(course: CreateCourse) : Promise<Course>{
  return (await postApiCall(`/courses`, course)).json()
}

export async function addTeacherToCourse(courseId: string, teacher: AddTeacherToCourse){
  return postApiCall(`/courses/${courseId}/teachers`, teacher)
}

export async function deleteCourse(courseId: string){
  return deleteApiCall(`/courses/${courseId}`)
}

export async function editCourse(courseId: string, course: CreateCourse) {
  return putApiCall(`/courses/${courseId}`, course)
}
