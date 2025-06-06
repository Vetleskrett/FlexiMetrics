import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditAnalyzer } from 'src/types/';
import { api } from 'src/api.server';

export const GET: RequestHandler = async ({ params }) => {
  const response = await api.get(`/analyzers/${params.analyzerId}`);
  return json(response.data, {
    status: response.status
  });
}

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditAnalyzer = await request.json()
  const response = await api.put(`/analyzers/${params.analyzerId}`, payload)
  return json(response.data, {
    status: response.status
  });
}

export const DELETE: RequestHandler = async ({ params }) => {
  const response = await api.delete(`/analyzers/${params.analyzerId}`)
  return json(response.data, {
    status: response.status
  });
}