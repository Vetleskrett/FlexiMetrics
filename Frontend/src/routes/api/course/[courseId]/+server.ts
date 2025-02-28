import { deleteCourse, editCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditCourse } from 'src/types';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditCourse = await request.json()
  const response = await editCourse(params.courseId as string, payload)
  return json(response.data)
}

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteCourse(params.courseId as string)
  return json(response.data)
}