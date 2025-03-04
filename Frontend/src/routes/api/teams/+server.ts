import { postTeams } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { CreateTeams } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateTeams = await request.json()
  const response = await postTeams(payload)
  return json(response.data)
}
