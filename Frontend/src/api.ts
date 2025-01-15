import type { Course } from "./types";
const API_BASE_URL = 'https://localhost:7255';

export async function getCourses() : Promise<Course[]> {
  try {
    const response = await fetch(`${API_BASE_URL}/courses`);
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
