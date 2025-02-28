import { deleteStudentTeam, postStudentTeam } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteStudentTeam(params.teamId as string, params.studentId as string)
  return json(response.data)
}

export const POST: RequestHandler = async ({ params }) => {
  const response = await postStudentTeam(params.teamId as string, params.studentId as string)
  return json(response.data)
}