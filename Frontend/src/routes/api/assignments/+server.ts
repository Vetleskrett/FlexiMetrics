import { postAssignment } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { CreateAssignment } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateAssignment = await request.json()
  const response = await postAssignment(payload)
  return json(response.data)
}
