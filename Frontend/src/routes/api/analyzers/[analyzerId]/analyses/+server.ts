import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const GET: RequestHandler = async ({ params }) => {
  const response = await api.get(`/analyzers/${params.analyzerId}/analyses`)
  return json(response.data, {
    status: response.status
  });
}
