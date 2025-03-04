import { postTeacherToCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EmailAdd } from 'src/types';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload: EmailAdd = await request.json()
  console.log(payload)
  const response = await postTeacherToCourse(params.courseId as string, payload)
  return json(response.data)
}
