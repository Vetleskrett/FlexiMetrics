import { json, type RequestHandler } from '@sveltejs/kit';
import type { UpdateDelivery } from 'src/types/';
import { api } from 'src/api.server';

export const PUT: RequestHandler = async ({ params, request }) => {
  const payload: UpdateDelivery = await request.json();
  const response = await api.put(`deliveries/${params.deliveryId}`, payload);  
  return json(response.data, {
    status: response.status
  });
}
