import { deleteAssigmentField} from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await deleteAssigmentField(params.assignmentFieldId as string)
  return json(response.data)
}