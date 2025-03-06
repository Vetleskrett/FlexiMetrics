import { json, type RequestHandler } from '@sveltejs/kit';
import type { AddStudentsToTeams } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: AddStudentsToTeams = await request.json();
  const response = await api.post(`teams/bulk`, payload);
  return json(response.data);
}
