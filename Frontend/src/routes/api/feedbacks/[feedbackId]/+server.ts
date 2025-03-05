import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditFeedback } from 'src/types';
import { api } from 'src/api.server';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditFeedback = await request.json();
  const response = await api.put(`feedbacks/${params.feedbackId}`, payload);
  return json(response.data);
}