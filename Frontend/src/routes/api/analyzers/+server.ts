import { json, type RequestHandler } from '@sveltejs/kit';
import type {CreateAnalyzer } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateAnalyzer = await request.json()
  const response = await api.post(`/analyzers`, payload)
  return json(response.data)
}
