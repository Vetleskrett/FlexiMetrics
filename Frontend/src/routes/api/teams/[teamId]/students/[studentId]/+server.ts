import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ params }) => {
  const response = await api.post(`teams/${params.teamId}/students/${params.studentId}`);
  return json(response.data)
}

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`teams/${params.teamId}/students/${params.studentId}`);
  return json(response.data)
}