import { deleteStudentCourse } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteStudentCourse(params.courseId as string, params.studentId as string)
  return json(response.data)
}
