import { publishAssignment } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';

export const PUT: RequestHandler = async ({ params }) => {
  const response = await publishAssignment(params.assignmentId as string)
  return json(response.data)
}