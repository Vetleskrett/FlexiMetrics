import { json, type RequestHandler } from '@sveltejs/kit';
import type { UpdateAssignmentFields } from 'src/types/';
import { api } from 'src/api.server';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: UpdateAssignmentFields = await request.json()
  const response = await api.put(`assignments/${params.assignmentId}/fields`, payload);
  return json(response.data, {
    status: response.status
  });
}