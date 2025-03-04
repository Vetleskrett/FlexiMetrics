import { putFeedback } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditFeedback } from 'src/types';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditFeedback = await request.json()
  const response = await putFeedback(params.feedbackId as string, payload)
  return json(response.data)
}