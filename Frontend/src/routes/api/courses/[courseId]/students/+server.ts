import { json, type RequestHandler } from '@sveltejs/kit';
import type { AddStudentsToCourse } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload: AddStudentsToCourse = await request.json();
  const response = await api.post(`courses/${params.courseId}/students`, payload);  
  return json(response.data);
}
