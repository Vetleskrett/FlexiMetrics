import { postCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { CreateCourse, EditCourse } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateCourse = await request.json()
  const response = await postCourse(payload)
  return json(response.data)
}
