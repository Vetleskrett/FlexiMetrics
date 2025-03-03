import { postStudentsTeam } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { AddStudentsToTeams } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: AddStudentsToTeams = await request.json()
  const response = await postStudentsTeam(payload)
  return json(response.data)
}
