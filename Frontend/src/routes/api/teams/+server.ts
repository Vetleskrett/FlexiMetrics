import { json, type RequestHandler } from '@sveltejs/kit';
import type { CreateTeams } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateTeams = await request.json();
  const response = await api.post(`teams`, payload);
  return json(response.data);
}
