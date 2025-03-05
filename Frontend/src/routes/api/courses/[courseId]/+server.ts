import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditCourse } from 'src/types';
import { api } from 'src/api.server';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditCourse = await request.json();
  const response = await api.put(`courses/${params.courseId}`, payload);  
  return json(response.data);
}

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`courses/${params.courseId}`);  
  return json(response.data);
}