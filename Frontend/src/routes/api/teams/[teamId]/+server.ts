import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`teams/${params.teamId}`);
  return json(response.data, {
    status: response.status
  });
}