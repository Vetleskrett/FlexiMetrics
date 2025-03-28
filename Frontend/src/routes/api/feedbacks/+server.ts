import { json, type RequestHandler } from '@sveltejs/kit';
import type {CreateFeedback } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateFeedback = await request.json();
  const response = await api.post(`feedbacks`, payload);
  return json(response.data, {
    status: response.status
  });
}
