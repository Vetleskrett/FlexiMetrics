import { addTeacherToCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EmailAdd } from 'src/types';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload: EmailAdd = await request.json()
  const response = await addTeacherToCourse(params.courseId as string, payload)
  return json(response.data)
}
