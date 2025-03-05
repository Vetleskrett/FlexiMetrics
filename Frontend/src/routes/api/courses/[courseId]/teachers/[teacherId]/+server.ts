import { api } from 'src/api.server';
import { json, type RequestHandler } from '@sveltejs/kit';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`courses/${params.courseId}/teachers/${params.teacherId}`);
  return json(response.data)
}
