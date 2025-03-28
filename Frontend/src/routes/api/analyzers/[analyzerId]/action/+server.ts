import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload = await request.json()
  const response = await api.post(`/analyzers/${params.analyzerId}/action`, payload)
  return json(response.data, {
    status: response.status
  });
}
