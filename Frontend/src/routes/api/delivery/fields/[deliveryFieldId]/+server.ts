import { getDeliveryFieldFile, postDeliveryFieldFile } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload = await request.formData()
  const response = await postDeliveryFieldFile(params.deliveryFieldId as string, payload)
  return json(response.data)
}

export const GET: RequestHandler = async ({ params }) => {
  const fileResponse = await getDeliveryFieldFile(params.deliveryFieldId as string)
  const headers = new Headers();

  headers.set('Content-Disposition', fileResponse.headers['content-disposition'] || 'attachment');
  headers.set('Content-Type', fileResponse.headers['content-type'] || 'application/octet-stream');
  headers.set('Content-Length', fileResponse.headers['content-length'] ||  '');
  
  return new Response(fileResponse.data, {
    status: 200,
    headers: headers,
  })
  
}
