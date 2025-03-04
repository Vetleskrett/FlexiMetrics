import { postDelivery } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type {CreateDelivery } from 'src/types';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateDelivery = await request.json()
  const response = await postDelivery(payload)
  return json(response.data)
}
