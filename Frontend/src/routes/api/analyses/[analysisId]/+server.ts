import { json, type RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';

export const GET: RequestHandler = async ({ params }) => {
  const response = await api.get(`/analyses/${params.analysisId}`);
  return json(response.data);
}
