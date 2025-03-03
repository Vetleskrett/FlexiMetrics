import { addAssignmentFields } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { RegisterAssignmentFields } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: RegisterAssignmentFields = await request.json()
  const response = await addAssignmentFields(payload)
  return json(response.data)
}
