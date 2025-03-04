import { postStudentsCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { AddStudentsToCourse } from 'src/types';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload: AddStudentsToCourse = await request.json()
  const response = await postStudentsCourse(params.courseId as string, payload)
  return json(response.data)
}
