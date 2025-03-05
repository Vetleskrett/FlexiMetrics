import { json, type RequestHandler } from '@sveltejs/kit';
import type { EmailAdd } from 'src/types';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload: EmailAdd = await request.json()
  const response = await api.post(`courses/${params.courseId}/teachers`, payload);  
  return json(response.data)
}
