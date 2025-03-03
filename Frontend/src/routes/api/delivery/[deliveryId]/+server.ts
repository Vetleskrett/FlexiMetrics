import { putDelivery } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { UpdateDelivery } from 'src/types';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: UpdateDelivery = await request.json()
  const response = await putDelivery(params.deliveryId as string, payload)
  return json(response.data)
}
