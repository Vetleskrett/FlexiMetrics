import { postFeedback } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type {CreateFeedback } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateFeedback = await request.json()
  const response = await postFeedback(payload)
  return json(response.data)
}
