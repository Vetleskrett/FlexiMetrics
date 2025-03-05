import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`courses/${params.courseId}/students/${params.studentId}`);
  return json(response.data);
}
