import { putAnalyzer } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EditAnalyzer } from 'src/types';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: EditAnalyzer = await request.json()
  const response = await putAnalyzer(params.analyzerId as string, payload)
  return json(response.data)
}
