import { deleteAssigmentField, editAssignmentFields} from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { UpdateAssignmentFields } from 'src/types';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteAssigmentField(params.assignmentFieldId as string)
  return json(response.data)
}

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: UpdateAssignmentFields = await request.json()
  const response = await editAssignmentFields(params.assignmentFieldId as string, payload)
  return json(response.data)
}