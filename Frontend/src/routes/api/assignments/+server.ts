import { json, type RequestHandler } from '@sveltejs/kit';
import type { CreateAssignment } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateAssignment = await request.json()
  const response = await api.post(`assignments`, payload);
  return json(response.data)
}
