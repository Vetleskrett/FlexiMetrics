import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const PUT: RequestHandler = async ({ params }) => {
  const response = await api.put(`assignments/${params.assignmentId}/publish`);
  return json(response.data);
}