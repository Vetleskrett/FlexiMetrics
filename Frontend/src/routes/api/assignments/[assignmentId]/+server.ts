import { deleteAssigment, editAssignment, putFeedback } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditAssignment, EditFeedback } from 'src/types';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditAssignment = await request.json()
  const response = await editAssignment(params.assignmentId as string, payload)
  return json(response.data)
}

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteAssigment(params.assignmentId as string)
  return json(response.data)
}