import { api } from 'src/api.server';
import { json, type RequestHandler } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ params }) => {
  const response = await api.get(`/analyzers/${params.analyzerId}/logs`);
  return json(response.data, {
    status: response.status
  });
}