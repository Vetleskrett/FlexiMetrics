import { postAnalyzer } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type {CreateAnalyzer } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateAnalyzer = await request.json()
  const response = await postAnalyzer(payload)
  return json(response.data)
}
