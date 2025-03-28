import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditAssignment } from 'src/types/';
import { api } from 'src/api.server';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditAssignment = await request.json()
  const response = await api.put(`assignments/${params.assignmentId}`, payload);
  return json(response.data, {
    status: response.status
  });
}

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`assignments/${params.assignmentId}`);
  return json(response.data, {
    status: response.status
  });
}