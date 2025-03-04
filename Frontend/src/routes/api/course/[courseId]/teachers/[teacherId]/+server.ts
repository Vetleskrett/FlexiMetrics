import { deleteTeacherFromCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteTeacherFromCourse(params.courseId as string, params.teacherId as string)
  return json(response.data)
}
