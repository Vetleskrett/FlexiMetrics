import { json, type RequestHandler } from '@sveltejs/kit';
import type { CreateCourse } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateCourse = await request.json();
  const response = await api.post(`courses`, payload);
  return json(response.data);
}
