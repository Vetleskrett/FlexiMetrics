import { postStudentEmailTeam } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EmailAdd } from 'src/types';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload: EmailAdd = await request.json()
  const response = await postStudentEmailTeam(params.teamId as string, payload)
  return json(response.data)
}