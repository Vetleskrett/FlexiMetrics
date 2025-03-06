import { json, type RequestHandler } from '@sveltejs/kit';
import type {CreateDelivery } from 'src/types/';
import { api } from 'src/api.server';

export const POST: RequestHandler = async ({ request }) => {
  const payload: CreateDelivery = await request.json();
  const response = await api.post(`deliveries`, payload);
  return json(response.data);
}
